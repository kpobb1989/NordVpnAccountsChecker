using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

using SeleniumUndetectedChromeDriver;

namespace NordVpnAccountsChecker.Components
{
    public static class ChromeDriverEx
    {
        public static IWebDriver CreateUndetectedChromeDriver(Dictionary<string, object>? prefs = null)
        {
            var driver = UndetectedChromeDriver.Create(prefs: prefs, driverExecutablePath: $@"{AppDomain.CurrentDomain.BaseDirectory}chromedriver.exe");

            driver.Manage().Window.Maximize();

            return driver;
        }
    }
}

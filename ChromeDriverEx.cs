using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

using SeleniumUndetectedChromeDriver;

namespace NordVpnAccountsChecker
{
    public static class ChromeDriverEx
    {
        public static IWebDriver CreateUndetectedChromeDriver(ChromeOptions? options = null)
        {
            var driver = UndetectedChromeDriver.Create(options: options, driverExecutablePath: $@"{AppDomain.CurrentDomain.BaseDirectory}chromedriver.exe");

            driver.Manage().Window.Maximize();

            return driver;
        }
    }
}

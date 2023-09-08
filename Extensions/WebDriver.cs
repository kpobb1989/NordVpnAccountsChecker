using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace NordVpnAccountsChecker.Extensions
{
    public static class WebDriver
    {
        public static IWebElement FindElementWithTimeout(this IWebDriver driver, By by)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            wait.Until(s =>
            {
                s.BypassCloudFlare();

                try
                {
                    return s.FindElement(by).Displayed;
                }
                catch { return false; }
            });

            return driver.FindElement(by);
        }

        public static void BypassCloudFlare(this IWebDriver driver)
        {
            try
            {
                var cloudFlareIframe = driver.FindElement(By.CssSelector("[id^='cf-chl-widget']"));

                if (cloudFlareIframe != null)
                {
                    var frame = driver.SwitchTo().Frame(cloudFlareIframe);

                    var spinner = frame.FindElementWithTimeout(By.XPath("//*[@id='spinner-icon']"));

                    while (true)
                    {
                        // Get the value of the "class" attribute
                        string classAttribute = driver.FindElement(By.Id("spinner-icon")).GetAttribute("class");

                        // Check if "unspun" class is present in the class attribute
                        if (classAttribute.Split(' ').Contains("unspun"))
                        {
                            var captcha = frame.FindElement(By.XPath("//*[@id=\"challenge-stage\"]/div/label/input"));

                            var actions = new Actions(driver);

                            // Move the mouse to the element and then click it
                            actions.MoveToElement(captcha).Perform();

                            // Perform a click action
                            actions.Click().Perform();

                            break;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public static void HandleTooManyRequests(this IWebDriver driver, string loginUrl)
        {
            while (true)
            {
                try
                {
                    var h1 = driver.FindElement(By.XPath("//*[@id=\"app\"]/div/div/div/div/div/div/h1"));

                    if (h1 != null && h1.Text == Message.TooManyRequests)
                    {
                        RandomWait(minSeconds: 300, maxSeconds: 600);

                        driver.Navigate().GoToUrl(loginUrl);
                    }
                    else
                    {
                        break;
                    }
                }
                catch
                {
                    break;
                }
            }
        }

        public static void EnterLoginAndSubmit(this IWebDriver driver, string login)
        {
            try
            {
                // Login
                driver.FindElementWithTimeout(By.XPath("//input[@data-testid='identifier-input']")).SendKeys(login);

                // Submit 
                driver.FindElementWithTimeout(By.XPath("//button[@data-testid='identifier-submit']")).Click();
            }
            catch
            {
            }
        }

        public static void EnterPasswordAndSignIn(this IWebDriver driver, string password)
        {
            try
            {
                // Password
                driver.FindElementWithTimeout(By.XPath("//input[@data-testid='signin-password-input']")).SendKeys(password);

                // Sign In
                driver.FindElementWithTimeout(By.XPath("//button[@data-testid='signin-button']")).Click();
            }
            catch
            {
            }
        }

        public static bool HandleMessage(this IWebDriver driver)
        {
            bool success = false;

            // Find an erorr message
            try
            {
                var message = driver.FindElementWithTimeout(By.XPath("//*[@id=\"app\"]/div/div/main/div[1]/div/p/span")).Text;

                if (message == Message.AccountBlocked || message == Message.IncorrectPassword)
                {
                    RandomWait(minSeconds: 10, maxSeconds: 30);
                }
                else if (message == Message.TooManyRequestsTryIn5Mins)
                {
                    Console.WriteLine("Waiting 5 mins");

                    Thread.Sleep(TimeSpan.FromSeconds(5 * 60 + 10));
                }
                else
                {
                    success = true;

                    RandomWait(minSeconds: 10, maxSeconds: 30);
                }
            }
            catch
            {
            }

            var text = driver.FindElementWithTimeout(By.XPath("//*[@id=\"app\"]/div/div/main/h1")).Text; 

            if(text == Message.AuthenticatorApp)
            {
                success = false;
            }

            // https://my.nordaccount.com/billing/my-subscriptions/
            // //*[@id="app"]/div[2]/div[2]/div/div[1]/div[3]/div/div/div[1]/p
            // No active subscriptions

            return success;
        }

        private static void RandomWait(int minSeconds = 1, int maxSeconds = 5)
        {
            var randomSeconds = Random.Shared.Next(minSeconds, maxSeconds);

            Console.WriteLine($"Waiting {randomSeconds} seconds");

            Thread.Sleep(randomSeconds * 1000);
        }
    }
}

using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.Events;
using System.Text.RegularExpressions;
using NUnit.Framework;
using NUnit.Allure.Core;
using NUnit.Allure.Attributes;

namespace NUnitSelenium
{
    // The webdriver setup and teardown all happen in the test file itself, in the OneTimeSetup and OneTimeTearDown.
    // This lets the test file use the same browser instance for all tests. This saves resources and processing, but
    // it is still best practice if those tests do not depend upon each other for state.
    public class WebDriverSetup : IDisposable
    {
        private static IWebDriver _driver;
        private static EventFiringWebDriver _firingDriver;


        public enum BrowserType
        {
            NotSet,
            Chrome,
            Firefox,
            Edge,
        }

        public WebDriverSetup()
        {
        }

        /* 
            There does not seem to be an event that fires when an Assert fails (so not trapped by _firingDriver_ExceptionThrown);
            however, wrapping the assert in a try/catch works, so call CaptureScreenshot to get the screen at time of failure and
            attach to the Allure report.
        */
        public bool CaptureScreenshot(IWebDriver driver, string testId, string testName, Allure.Commons.AllureLifecycle _lifecycle)
        {
            try
            {
                // Get the screenshot from Selenium WebDriver and save it to a file
                Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
                string ssName = testName
                    // + testId
                    + DateTime.Now.ToString("yyyyMMddTHHmmss");
                ssName = Regex.Replace(ssName, @"[^0-9a-zA-Z]+", "") + ".png";
                string screenshotFile = _lifecycle.ResultsDirectory + "\\" + ssName;
                ss.SaveAsFile(screenshotFile, ScreenshotImageFormat.Png);

                // Add that file to NUnit results
                _lifecycle.AddAttachment(screenshotFile);

                return true;
            }
            catch
            {
                return false;
            }
        }


        public static IWebDriver Create_Browser(BrowserType browserType)
        {

            switch (browserType)
            {
                case BrowserType.Chrome:
                    ChromeOptions cOptions = new ChromeOptions();
                    cOptions.AddArgument("ignore-certificate-errors");
                    cOptions.AddArgument("window-size=1900x1080");
                    cOptions.AddArgument("start-maximized");

                    // set logging to "Severe" and the event will add anything from the js console to the allure report
                    cOptions.SetLoggingPreference(LogType.Browser, LogLevel.All);
                    // cOptions.SetLoggingPreference(LogType.Driver, LogLevel.All);
                    _driver = new ChromeDriver(cOptions);
                    _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
                    _firingDriver = new EventFiringWebDriver(_driver);
                    _firingDriver.ExceptionThrown += _firingDriver_ExceptionThrown; 
                    _firingDriver.Navigated += _firingDriver_Navigated;
                    return _firingDriver; 
                case BrowserType.Firefox:
                    FirefoxOptions options = new FirefoxOptions();
                    options.AddArgument("ignore-certificate-errors");
                    // logging does not appear to work for Firefox... 
                    options.SetLoggingPreference(LogType.Browser, LogLevel.All);
                    _driver = new FirefoxDriver(options);
                    _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
                    _firingDriver = new EventFiringWebDriver(_driver);
                    _firingDriver.ExceptionThrown += _firingDriver_ExceptionThrown; ;
                    _firingDriver.Navigated += _firingDriver_Navigated;
                    return _firingDriver;
                case BrowserType.Edge:
                    return new EdgeDriver();
                default:
                    throw new ArgumentOutOfRangeException(nameof(browserType), browserType, null);
            }
        }

        private static void _firingDriver_Navigated(object sender, WebDriverNavigationEventArgs e)
        {
            // output sent to the console is trapped and included in the Allure report
            var entries = e.Driver.Manage().Logs.GetLog(LogType.Browser);
            foreach (var entry in entries)
            {
                Console.WriteLine(entry.ToString());
            }
        }

        private static void _firingDriver_ExceptionThrown(object sender, WebDriverExceptionEventArgs e)
        {
            Console.WriteLine(sender.ToString());
            Console.WriteLine(e.ThrownException.Message);
        }

        public void Dispose()
        {
            _driver.Dispose();
            _firingDriver.Dispose();
        }
    }

}
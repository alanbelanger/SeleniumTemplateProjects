using System;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.Events;
using System.Text.RegularExpressions;
using NUnit.Framework;
using NUnit.Allure.Core;
using NUnit.Allure.Attributes;
using System.Threading;

[assembly: Parallelizable(ParallelScope.Fixtures)]
[assembly: LevelOfParallelism(6)]

namespace NUnitSelenium
{
    /// <summary>
    /// The webdriver setup and teardown all happen in the test file itself, in the OneTimeSetup and OneTimeTearDown.
    /// 
    /// This lets the test file use the same browser instance for all tests. This saves resources and processing, 
    /// but it is still best practice if those tests do not depend upon each other for state. 
    /// </summary>
    public class WebDriverSetup : IDisposable
    {
        private static ThreadLocal<IWebDriver> driver = new ThreadLocal<IWebDriver>();
        private static ThreadLocal<EventFiringWebDriver> firingDriver = new ThreadLocal<EventFiringWebDriver>();

        /// <summary>
        /// List of all supported browsers.
        /// </summary>
        public enum BrowserType
        {
            NotSet,
            Chrome,
            Firefox,
            Edge,
            GalaxyA51
        }

        public WebDriverSetup()
        {
        }

        /// <summary>
        /// Take a screenshot, save to a file and attach to allure report.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="testId"></param>
        /// <param name="testName"></param>
        /// <param name="lifecycle"></param>
        /// <returns></returns>
        public bool CaptureScreenshot(IWebDriver driver, string testId, string testName, Allure.Commons.AllureLifecycle lifecycle)
        {
            try
            {
                // Get the screenshot from Selenium WebDriver and save it to a file
                Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
                string ssName = testName
                    // + testId
                    + DateTime.Now.ToString("yyyyMMddTHHmmss");
                ssName = Regex.Replace(ssName, @"[^0-9a-zA-Z]+", "") + ".png";
                string screenshotFile = lifecycle.ResultsDirectory + "\\" + ssName;
                ss.SaveAsFile(screenshotFile, ScreenshotImageFormat.Png);

                // Add that file to NUnit results
                lifecycle.AddAttachment(screenshotFile);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Take a screenshot, save to a file and make accessible to NUnit's results.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="testId"></param>
        /// <param name="testName"></param>
        /// <param name="testContext"></param>
        /// <returns></returns>
        public bool CaptureScreenshot(IWebDriver driver, string testId, string testName, TestContext testContext)
        {
            try
            {
                // Get the screenshot from Selenium WebDriver and save it to a file
                Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
                string ssName = testName
                    // + testId
                    + DateTime.Now.ToString("yyyyMMddTHHmmss");
                ssName = Regex.Replace(ssName, @"[^0-9a-zA-Z]+", "") + ".png";
                string screenshotFile = Path.Combine(TestContext.CurrentContext.WorkDirectory, ssName);
                ss.SaveAsFile(screenshotFile, ScreenshotImageFormat.Png);

                // Add that file to NUnit results
                TestContext.AddTestAttachment(screenshotFile);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// CreateBrowser returns an IWebBrowser object to be used by page object classes.
        /// </summary>
        /// <param name="browserType"></param>
        /// <returns></returns>
        public static IWebDriver CreateBrowser(BrowserType browserType)
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
                    driver.Value = new ChromeDriver(cOptions);
                    driver.Value.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
                    firingDriver.Value = new EventFiringWebDriver(driver.Value);
                    firingDriver.Value.ExceptionThrown += _firingDriver_ExceptionThrown; 
                    firingDriver.Value.Navigated += _firingDriver_Navigated;
                    return firingDriver.Value; 
                case BrowserType.Firefox:
                    FirefoxOptions options = new FirefoxOptions();
                    options.AddArgument("ignore-certificate-errors");
                    // logging does not appear to work for Firefox... 
                    options.SetLoggingPreference(LogType.Browser, LogLevel.All);
                    driver.Value = new FirefoxDriver(options);
                    driver.Value.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
                    firingDriver.Value = new EventFiringWebDriver(driver.Value);
                    firingDriver.Value.ExceptionThrown += _firingDriver_ExceptionThrown; ;
                    firingDriver.Value.Navigated += _firingDriver_Navigated;
                    return firingDriver.Value;
                case BrowserType.Edge:
                    return new EdgeDriver();
                case BrowserType.GalaxyA51:
                    ChromeOptions chromeOptions = new ChromeOptions();
                    chromeOptions.AddAdditionalChromeOption("androidPackage", "com.android.chrome");
                    chromeOptions.AddAdditionalChromeOption("androidDeviceSerial", "");
                    driver.Value = new ChromeDriver(chromeOptions);
                    driver.Value.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
                    firingDriver.Value = new EventFiringWebDriver(driver.Value);
                    firingDriver.Value.ExceptionThrown += _firingDriver_ExceptionThrown;
                    firingDriver.Value.Navigated += _firingDriver_Navigated;
                    return firingDriver.Value;
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
            driver.Dispose();
            firingDriver.Dispose();
        }
    }

}
using System;
using System.Collections.ObjectModel;
using OpenQA.Selenium;
using NUnit.Framework;
using NUnit.Allure.Core;
using NUnit.Allure.Attributes;


namespace NUnitSelenium
{
    [TestFixture(WebDriverSetup.BrowserType.Chrome)]
    [TestFixture(WebDriverSetup.BrowserType.Firefox)]
    //[TestFixture(WebDriverSetup.BrowserType.GalaxyA51)]
    [AllureNUnit]
    [AllureSuite("ZeroBankTests")]
    class AuthenticatedPageTests
    {
        private static WebDriverSetup.BrowserType myBrowser;
        private static WebDriverSetup oDriverSetup;
        private static IWebDriver driver;
        private static AuthenticatedPages authenticatedPages;
        private static String baseurl = "http://zero.webappsecurity.com/";
        private static Allure.Commons.AllureLifecycle lifecycle;

        public AuthenticatedPageTests(WebDriverSetup.BrowserType browser)
        {
            myBrowser = browser;
            lifecycle = Allure.Commons.AllureLifecycle.Instance;
            oDriverSetup = new WebDriverSetup();
        }

        [OneTimeSetUp]
        public static void WebdriverSetup()
        {
            driver = WebDriverSetup.CreateBrowser(myBrowser);
            authenticatedPages = new AuthenticatedPages(driver);
        }

        [Test]
        [AllureSubSuite("Authenticated pages")]
        [TestCase("freddy", "mercury")] //failure case
        [TestCase("username", "password")]
        public static void TestSigninValues(string user, string pass)
        {
            authenticatedPages.GoToZeroBank(baseurl);
            if (!authenticatedPages.ClickGotoSigninPage())
            {
                authenticatedPages.Logout();
                authenticatedPages.ClickGotoSigninPage();
            }
            authenticatedPages.FillLoginForm(user, pass);
            try
            {
                Assert.IsTrue(driver.Url == "http://zero.webappsecurity.com/bank/account-summary.html");
                authenticatedPages.ClickGetStatement();
                Assert.IsTrue(authenticatedPages.GetStatementName() == "Statement 01/10/12(57K)");
                authenticatedPages.Logout();
            }
            catch (AssertionException)
            {
                // Catch the assert to allow capturing the screenshot on failure only.
                // Be sure to re-throw explitly after taking the screenshot to ensure the test rightly fails.
                oDriverSetup.CaptureScreenshot(driver, TestContext.CurrentContext.Test.ID, TestContext.CurrentContext.Test.Name, lifecycle);
                throw;
            }

        }

        [Test]
        [AllureSubSuite("Authenticated pages")]
        [TestCase("username", "password")]
        public static void TestAccountActivity(string user, string pass)
        {
            authenticatedPages.GoToZeroBank(baseurl);
            if (!authenticatedPages.ClickGotoSigninPage())
            {
                authenticatedPages.Logout();
                authenticatedPages.ClickGotoSigninPage();
            }
            try
            {
                authenticatedPages.FillLoginForm(user, pass);
                Assert.IsTrue(driver.Url == "http://zero.webappsecurity.com/bank/account-summary.html");
                ReadOnlyCollection<IWebElement> lResults = authenticatedPages.GetAccountActivity();
                Assert.True(lResults[1].Text == "ONLINE TRANSFER REF #UKKSDRQG6L");
                authenticatedPages.Logout();
            }
            catch (AssertionException)
            {
                oDriverSetup.CaptureScreenshot(driver, TestContext.CurrentContext.Test.ID, TestContext.CurrentContext.Test.Name, lifecycle);
                throw;
            }

        }

        [OneTimeTearDown]
        public void Close()
        {
            driver.Quit();
        }
    }

}

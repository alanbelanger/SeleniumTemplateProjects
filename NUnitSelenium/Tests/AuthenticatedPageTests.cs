using System;
using System.Collections.ObjectModel;
using System.IO;
using OpenQA.Selenium;
using NUnit.Framework;
using NUnit.Allure.Core;
using NUnit.Allure.Attributes;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NUnitSelenium
{
    //[Ignore("testing parallel")]
    [TestFixture(WebDriverSetup.BrowserType.Chrome)]
    //[TestFixture(WebDriverSetup.BrowserType.Firefox)]
    //[TestFixture(WebDriverSetup.BrowserType.GalaxyA51)]
    [AllureNUnit]
    [AllureSuite("ZeroBankTests")]
    [AllureDisplayIgnored]
    class AuthenticatedPageTests
    {
        private static WebDriverSetup.BrowserType myBrowser;
        private static WebDriverSetup oDriverSetup;
        private static IWebDriver driver;
        private static AuthenticatedPages authenticatedPages;
        private static String baseurl = "http://zero.webappsecurity.com/";
        private static Allure.Commons.AllureLifecycle lifecycle;
        private static JObject myJsonData;

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

        private static IEnumerable<TestCaseData> AddUsersFromJson()
        {
            String DirectoryName = TestContext.CurrentContext.TestDirectory;
            var myTestData = File.ReadAllText(DirectoryName + "\\testData.json");
            myJsonData = JObject.Parse(myTestData);

            foreach (JToken user in myJsonData.SelectToken("..loginForm"))
            {
                string username = user["username"].ToString();
                string password = user["password"].ToString();
                yield return new TestCaseData(username, password);
            }
            
        }

        [AllureSubSuite("Authenticated pages")]
        //[TestCase("freddy", "mercury")] //failure case
        //[TestCase("username", "password")]
        [TestCaseSource(nameof(AddUsersFromJson))]
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
                oDriverSetup.CaptureScreenshot(driver, TestContext.CurrentContext.Test.ID, TestContext.CurrentContext.Test.Name, TestContext.CurrentContext);
                throw;
            }

        }

        [AllureSubSuite("Authenticated pages")]
        [TestCaseSource(nameof(AddUsersFromJson))]
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
                oDriverSetup.CaptureScreenshot(driver, TestContext.CurrentContext.Test.ID, TestContext.CurrentContext.Test.Name, TestContext.CurrentContext);
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

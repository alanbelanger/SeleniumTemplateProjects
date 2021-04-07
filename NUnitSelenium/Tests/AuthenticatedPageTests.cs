﻿using System;
using System.Collections.ObjectModel;
using OpenQA.Selenium;
using NUnit.Framework;
using NUnit.Allure.Core;
using NUnit.Allure.Attributes;


namespace NUnitSelenium
{
    [TestFixture(WebDriverSetup.BrowserType.Chrome)]
    [TestFixture(WebDriverSetup.BrowserType.Firefox)]
    [AllureNUnit]
    [AllureSuite("ZeroBankTests")]
    class AuthenticatedPageTests
    {
        private static WebDriverSetup.BrowserType _mybrowser;
        private static WebDriverSetup _oDriverSetup;
        private static IWebDriver driver;
        private static AuthenticatedPages _authenticatedPages;
        private static String baseurl = "http://zero.webappsecurity.com/";
        private static Allure.Commons.AllureLifecycle _lifecycle;

        public AuthenticatedPageTests(WebDriverSetup.BrowserType browser)
        {
            _mybrowser = browser;
            _lifecycle = Allure.Commons.AllureLifecycle.Instance;
            _oDriverSetup = new WebDriverSetup();
        }

        [OneTimeSetUp]
        public static void WebdriverSetup()
        {
            driver = WebDriverSetup.Create_Browser(_mybrowser);
            _authenticatedPages = new AuthenticatedPages(driver);
        }

        [Test]
        [AllureSubSuite("Authenticated pages")]
        [TestCase("freddy","mercury")] //failure case
        [TestCase("username", "password")]
        public static void TestSigninValues(string user, string pass)
        {
            _authenticatedPages.GoToZeroBank(baseurl);
            _authenticatedPages.ClickGotoSigninPage();
            _authenticatedPages.FillLoginForm(user, pass);
            try
            {
                Assert.IsFalse(driver.Url == "http://zero.webappsecurity.com/login.html?login_error=true");
                _authenticatedPages.ClickGetStatement();
                Assert.IsTrue(_authenticatedPages.GetStatementName() == "Statement 01/10/12(57K)");
                _authenticatedPages.Logout();
            }
            catch
            {
                _oDriverSetup.CaptureScreenshot(driver, TestContext.CurrentContext.Test.ID, TestContext.CurrentContext.Test.Name, _lifecycle);
            }

        }

        [Test]
        [AllureSubSuite("Authenticated pages")]
        [TestCase("username", "password")]
        public static void TestAccountActivity(string user, string pass)
        {
            try
            {
                _authenticatedPages.GoToZeroBank(baseurl);
                _authenticatedPages.ClickGotoSigninPage();
                _authenticatedPages.FillLoginForm(user, pass);
                Assert.IsFalse(driver.Url == "http://zero.webappsecurity.com/login.html?login_error=true");
                ReadOnlyCollection<IWebElement> lResults = _authenticatedPages.GetAccountActivity();
                Assert.True(lResults[1].Text == "ONLINE TRANSFER REF #UKKSDRQG6L");
                _authenticatedPages.Logout();
            }
            catch(Exception ex)
            {
                _oDriverSetup.CaptureScreenshot(driver, TestContext.CurrentContext.Test.ID, TestContext.CurrentContext.Test.Name, _lifecycle);
                Assert.Fail(ex.Message);
            }
        }

        [OneTimeTearDown]
        public void Close()
        {
            driver.Quit();
        }
    }

}
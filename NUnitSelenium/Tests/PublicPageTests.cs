using System;
using NUnit.Framework;
using OpenQA.Selenium;
using NUnit.Allure.Core;
using NUnit.Allure.Attributes;

namespace NUnitSelenium
{
    /// <summary>
    /// Page Object class used by tests.
    /// </summary>
    [TestFixture(WebDriverSetup.BrowserType.Chrome)]
    //[TestFixture(WebDriverSetup.BrowserType.Firefox)]
    //[TestFixture(WebDriverSetup.BrowserType.GalaxyA51)]
    [AllureNUnit]
    [AllureSuite("ZeroBankTests")]
    public class PublicPageTests
    {
        private static PublicPages publicPages;
        private static WebDriverSetup.BrowserType myBrowser;
        private static IWebDriver driver;
        private static String baseurl = "http://zero.webappsecurity.com/";
        
        public PublicPageTests(WebDriverSetup.BrowserType browser)
        {
            myBrowser = browser;
        }

        [OneTimeSetUp]
        public static void WebdriverSetup()
        {
            driver = WebDriverSetup.CreateBrowser(myBrowser);
            publicPages = new PublicPages(driver);
        }

        [Test]
        [AllureSubSuite("Public pages")]
        public void TestSiteSearch()
        {
            publicPages.GoToZeroBank(baseurl);
            publicPages.SearchForText("account");
            int cellcount = publicPages.SearchResultCount();
            Assert.IsTrue(cellcount == 2);
        }

        [Test]
        [AllureSubSuite("Public pages")]
        public void TestTitle()
        {
            publicPages.GoToZeroBank(baseurl);
            Assert.IsTrue(driver.Title == "Zero - Personal Banking - Loans - Credit Cards");
        }

        [OneTimeTearDown]
        public void Close()
        {
            driver.Quit();
        }
    }

}

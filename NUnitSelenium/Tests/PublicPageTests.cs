using System;
using NUnit.Framework;
using OpenQA.Selenium;
using NUnit.Allure.Core;
using NUnit.Allure.Attributes;

namespace NUnitSelenium
{
    [TestFixture(WebDriverSetup.BrowserType.Chrome)]
    [TestFixture(WebDriverSetup.BrowserType.Firefox)]
    [AllureNUnit]
    [AllureSuite("ZeroBankTests")]
    //[AllureDisplayIgnored]
    public class PublicPageTests
    {
        private static PublicPages _publicPages;
        private static WebDriverSetup.BrowserType _mybrowser;
        private static IWebDriver driver;
        private static String baseurl = "http://zero.webappsecurity.com/";
        
        public PublicPageTests(WebDriverSetup.BrowserType browser)
        {
            _mybrowser = browser;
        }

        [OneTimeSetUp]
        public static void WebdriverSetup()
        {
            driver = WebDriverSetup.Create_Browser(_mybrowser);
            _publicPages = new PublicPages(driver);
        }

        [Test]
        [AllureTag("a tag")]
        [AllureSubSuite("Public pages")]
        public void TestSiteSearch()
        {
            _publicPages.GoToZeroBank(baseurl);
            _publicPages.SearchForText("account");
            int cellcount = _publicPages.SearchResultCount();
            Assert.IsTrue(cellcount == 2);
        }

        [Test]
        [AllureSubSuite("Public pages")]
        public void TestTitle()
        {
            _publicPages.GoToZeroBank(baseurl);
            Assert.IsTrue(driver.Title == "Zero - Personal Banking - Loans - Credit Cards");
        }

        [OneTimeTearDown]
        public void Close()
        {
            driver.Quit();
        }
    }

}

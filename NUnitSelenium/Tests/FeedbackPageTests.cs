﻿using System;
using NUnit.Framework;
using OpenQA.Selenium;
using NUnit.Allure.Core;
using NUnit.Allure.Attributes;

namespace NUnitSelenium
{
    //[TestFixture(WebDriverSetup.BrowserType.Chrome)]
    //[TestFixture(WebDriverSetup.BrowserType.Firefox)]
    [TestFixture(WebDriverSetup.BrowserType.GalaxyA51)]
    [AllureNUnit]
    [AllureSuite("ZeroBankTests")]
    class FeedbackPageTests
    {
        private static FeedbackPage feedbackPage;
        private static WebDriverSetup.BrowserType myBrowser;
        private static WebDriverSetup oDriverSetup; 
        private static IWebDriver driver;
        private static Allure.Commons.AllureLifecycle lifecycle;

        public FeedbackPageTests(WebDriverSetup.BrowserType browser)
        {
            myBrowser = browser;
            lifecycle = Allure.Commons.AllureLifecycle.Instance;
            oDriverSetup = new WebDriverSetup();
        }


        [OneTimeSetUp]
        public static void WebdriverSetup()
        {
            driver = WebDriverSetup.CreateBrowser(myBrowser);
            feedbackPage = new FeedbackPage(driver);
        }

        [Test]
        [AllureSubSuite("Public pages")]
        public void TestSubmitFill()
        {
            feedbackPage.GoToFeedbackForm();
            feedbackPage.FillForm("charlie", "charlie@angel.com", "i love money", "I do love money... why is it disappearing?");
            string sResult = feedbackPage.SendMessage();
            Assert.True(sResult == "http://zero.webappsecurity.com/sendFeedback.html");
        }

        [Test]
        [AllureTag("a tag")]
        [AllureSubSuite("Public pages")]
        [TestCase("charlie", "charlie@angel.com", "i love money", "I do love money... why is it disappearing?")]
        public void TestFillForm(string name, string email, string subject, string comment)
        {
            feedbackPage.GoToFeedbackForm();
            feedbackPage.FillForm(name, email, subject, comment);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(name, feedbackPage.Name);
                Assert.AreEqual(email, feedbackPage.Email);
                Assert.AreEqual(subject, feedbackPage.Subject);
                Assert.AreEqual(comment, feedbackPage.Comment);
            });

        }

        [Test]
        [AllureTag("a tag")]
        [AllureSubSuite("Public pages")]
        [TestCase("charlie", "charlie@angel.com", "i love money", "I do love money... why is it disappearing?")]
        public void TestClearForm(string name, string email, string subject, string comment)
        {
            feedbackPage.GoToFeedbackForm();
            feedbackPage.FillForm(name, email, subject, comment);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(name, feedbackPage.Name);
                Assert.AreEqual(email, feedbackPage.Email);
                Assert.AreEqual(subject, feedbackPage.Subject);
                Assert.AreEqual(comment, feedbackPage.Comment);
            });
            // if anything in above Assert.Multiple fails, you won't get past here

            feedbackPage.ClearForm();

            Assert.Multiple(() =>
            {
                Assert.AreEqual("", feedbackPage.Name);
                Assert.AreEqual("", feedbackPage.Email);
                Assert.AreEqual("", feedbackPage.Subject);
                Assert.AreEqual("", feedbackPage.Comment);
            });

        }

        [OneTimeTearDown]
        public void Close()
        {
            driver.Quit();
        }
    }
}

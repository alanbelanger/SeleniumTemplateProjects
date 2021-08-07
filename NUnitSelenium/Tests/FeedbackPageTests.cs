using System;
using NUnit.Framework;
using OpenQA.Selenium;
using NUnit.Allure.Core;
using NUnit.Allure.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NUnitSelenium
{
    [TestFixture(WebDriverSetup.BrowserType.Chrome)]
    //[TestFixture(WebDriverSetup.BrowserType.Firefox)]
    //[TestFixture(WebDriverSetup.BrowserType.GalaxyA51)]
    [AllureNUnit]
    [AllureSuite("ZeroBankTests")]
    [AllureDisplayIgnored]
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

        private static IEnumerable<TestCaseData> AddUsersFromJson()
        {
            System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            var myTestData = File.ReadAllText(TestContext.CurrentContext.WorkDirectory + "\\testData.json");
            
            JObject myJsonData = JObject.Parse(myTestData);

            foreach (JToken user in myJsonData.SelectToken("..feedbackForm"))
            {
                string name = user["name"].ToString();
                string email = user["email"].ToString();
                string subject = user["subject"].ToString();
                string comment = user["comment"].ToString();
                yield return new TestCaseData(name, email, subject, comment);
            }

        }

        private static IEnumerable<TestCaseData> AddFeedbackFromCSV()
        {
            string line;
            var assembly = Assembly.GetExecutingAssembly();
            Stream csvFile = assembly.GetManifestResourceStream("NUnitZeroBank.FeedbackFormData.csv"); //assembly.GetManifestResourceNames()
            StreamReader file = new StreamReader(csvFile);

            while (!file.EndOfStream)
            {
                line = file.ReadLine();
                string[] fields = line.Split(',');
                string name = fields[0];
                string email = fields[1];
                string subject = fields[2];
                string comments = fields[3];
                yield return new TestCaseData(name, email, subject, comments);
            }
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

        [AllureSubSuite("Public pages")]
        [Test, TestCaseSource(nameof(AddUsersFromJson))]
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

        [AllureSubSuite("Public pages")]
        [Test, TestCaseSource(nameof(AddFeedbackFromCSV))]
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

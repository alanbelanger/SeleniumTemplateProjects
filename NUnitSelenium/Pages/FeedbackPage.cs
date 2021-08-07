using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace NUnitSelenium
{
    internal class FeedbackPage
    {

            // Define the elements on the page, but keep them private. You will use these objects in the functions below,
            // and your tests will access the elements through these page functions.
            private IWebDriver currentDriver;

            private string url = "http://zero.webappsecurity.com/feedback.html";
            private IWebElement name => currentDriver.FindElement(By.Id("name"));
            private IWebElement email => currentDriver.FindElement(By.Id("email"));
            private IWebElement subject => currentDriver.FindElement(By.Id("subject"));
            private IWebElement comment => currentDriver.FindElement(By.Id("comment"));
            private IWebElement sendMessage => currentDriver.FindElement(By.CssSelector("input.btn-signin"));
            private IWebElement clearButton => currentDriver.FindElement(By.CssSelector("input.btn[type='reset']"));
            private IList<IWebElement> searchresults => currentDriver.FindElements(By.CssSelector("div.top_offset>ul>li"));

            public string Name
            {
                get { return name.GetAttribute("value"); }
                set { name.SendKeys(value); }
            }
            public string Email
            {
                get { return email.GetAttribute("value"); }
                set { email.SendKeys(value); }
            }
            public string Subject
            {
                get { return subject.GetAttribute("value"); }
                set { subject.SendKeys(value); }
            }
            public string Comment
            {
                get { return comment.GetAttribute("value"); }
                set { comment.SendKeys(value); }
            }
            public FeedbackPage(IWebDriver driver)
            {
                currentDriver = driver;
            }
            public void FillForm(string name, string email, string subject, string comment)
            {
                this.name.SendKeys(name);
                this.email.SendKeys(email);
                this.subject.SendKeys(subject);
                this.comment.SendKeys(comment);
            }
            public string SendMessage()
            {
                sendMessage.Click();
                return currentDriver.Url;
            }
            public bool ClearForm()
            {
                clearButton.Click();
                return true;
            }
            public bool GoToFeedbackForm()
            {
                currentDriver.Navigate().GoToUrl(url);
                if (currentDriver.Url == url)
                    return true;
                else
                    return false;
            }

    }
}

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

            public string Name   // property
            {
                get { return name.GetAttribute("value"); }   // get method
                set { name.SendKeys(value); }  // set method
            }
            public string Email // property
            {
                get { return email.GetAttribute("value"); }   // get method
                set { email.SendKeys(value); }  // set method
            }
            public string Subject // property
            {
                get { return subject.GetAttribute("value"); }   // get method
                set { subject.SendKeys(value); }  // set method
            }
            public string Comment   // property
            {
                get { return comment.GetAttribute("value"); }   // get method
                set { comment.SendKeys(value); }  // set method
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OpenQA.Selenium;

namespace NUnitSelenium
{
    internal class AuthenticatedPages
    {
        private IWebDriver currentDriver;
        private IWebElement signInButton => currentDriver.FindElement(By.CssSelector("button#signin_button"));
        private IWebElement nameField => currentDriver.FindElement(By.Name("user_login"));
        private IWebElement passwordField => currentDriver.FindElement(By.Name("user_password"));
        private IWebElement submitCredentialsButton => currentDriver.FindElement(By.Name("submit"));
        private IWebElement checkboxRememberMe => currentDriver.FindElement(By.Id("user_remember_me"));
        private IWebElement tabOnlineStatements => currentDriver.FindElement(By.Id("online_statements_tab"));
        private IWebElement tabAccountActivity => currentDriver.FindElement(By.Id("account_activity_tab"));
        private ReadOnlyCollection<IWebElement> listAccountActivities => currentDriver.FindElements(By.XPath("//div[@id='all_transactions_for_account']/table/tbody/tr/td"));
        private IWebElement statementName => currentDriver.FindElement(By.CssSelector("a[href*='.pdf']"));
        private ReadOnlyCollection<IWebElement> logoutDropdown => currentDriver.FindElements(By.ClassName("dropdown-toggle"));
        private IWebElement logoutLink => currentDriver.FindElement(By.CssSelector("a#logout_link"));

        public AuthenticatedPages(IWebDriver driver)
        {
            currentDriver = driver;
        }

        public bool GoToZeroBank(string url)
        {
            currentDriver.Navigate().GoToUrl(url);
            currentDriver.Manage().Cookies.DeleteAllCookies();
            if (currentDriver.Url == url)
                return true;
            else
                return false;
        }
        public bool ClickGotoSigninPage()
        {
            try
            {
                signInButton.Click();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool FillLoginForm(string user, string pass)
        {
            try
            {
                nameField.SendKeys(user);
                passwordField.SendKeys(pass);
                if (checkboxRememberMe.GetAttribute("checked") != "false")
                {
                    checkboxRememberMe.Click();
                }
                submitCredentialsButton.Click();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ClickGetStatement()
        {
            tabOnlineStatements.Click();
            return true;
        }

        public string GetStatementName()
        {
            return statementName.Text;
        }

        public ReadOnlyCollection<IWebElement> GetAccountActivity()
        {
            tabAccountActivity.Click();
            return listAccountActivities;
        }

        public bool Logout()
        {
            logoutDropdown[1].Click();
            logoutLink.Click();
            return true;
        }
    }
}

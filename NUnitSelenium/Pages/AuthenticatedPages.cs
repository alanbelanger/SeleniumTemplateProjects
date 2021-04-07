using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OpenQA.Selenium;

namespace NUnitSelenium
{
    internal class AuthenticatedPages
    {
        private IWebDriver _currentDriver;
        private IWebElement _signInButton => _currentDriver.FindElement(By.CssSelector("button#signin_button"));
        private IWebElement _nameField => _currentDriver.FindElement(By.Name("user_login"));
        private IWebElement _passwordField => _currentDriver.FindElement(By.Name("user_password"));
        private IWebElement _submitCredentialsButton => _currentDriver.FindElement(By.Name("submit"));
        private IWebElement _checkboxRememberMe => _currentDriver.FindElement(By.Id("user_remember_me"));
        private IWebElement _tabOnlineStatements => _currentDriver.FindElement(By.Id("online_statements_tab"));
        private IWebElement _tabAccountActivity => _currentDriver.FindElement(By.Id("account_activity_tab"));
        private ReadOnlyCollection<IWebElement> _listAccountActivities => _currentDriver.FindElements(By.XPath("//div[@id='all_transactions_for_account']/table/tbody/tr/td"));
        private IWebElement _statementName => _currentDriver.FindElement(By.CssSelector("a[href*='.pdf']"));
        private ReadOnlyCollection<IWebElement> _logoutDropdown => _currentDriver.FindElements(By.ClassName("dropdown-toggle"));
        private IWebElement _logoutLink => _currentDriver.FindElement(By.CssSelector("a#logout_link"));

        public AuthenticatedPages(IWebDriver driver)
        {
            _currentDriver = driver;
        }

        public bool GoToZeroBank(string url)
        {
            _currentDriver.Navigate().GoToUrl(url);
            if (_currentDriver.Url == url)
                return true;
            else
                return false;
        }
        public bool ClickGotoSigninPage()
        {
            try
            {
                _signInButton.Click();
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
                _nameField.SendKeys(user);
                _passwordField.SendKeys(pass);
                if (_checkboxRememberMe.GetAttribute("checked") != "true")
                {
                    _checkboxRememberMe.Click();
                }
                _submitCredentialsButton.Click();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ClickGetStatement()
        {
            _tabOnlineStatements.Click();
            return true;
        }

        public string GetStatementName()
        {
            return _statementName.Text;
        }

        public ReadOnlyCollection<IWebElement> GetAccountActivity()
        {
            _tabAccountActivity.Click();
            return _listAccountActivities;
        }

        public bool Logout()
        {
            _logoutDropdown[1].Click();
            _logoutLink.Click();
            return true;
        }
    }
}

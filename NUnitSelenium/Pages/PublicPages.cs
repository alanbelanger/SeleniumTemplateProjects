using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace NUnitSelenium
{
    internal class PublicPages
    {
        private IWebDriver _currentDriver;
        private IWebElement _searchBox => _currentDriver.FindElement(By.Id("searchTerm"));
        private IList<IWebElement> _searchResults => _currentDriver.FindElements(By.CssSelector("div.top_offset>ul>li"));

        public PublicPages(IWebDriver driver)
        {
            _currentDriver = driver;
        }
        public void SearchForText(string name)
        {
            _searchBox.SendKeys(name);
            _searchBox.SendKeys(Keys.Enter);
        }
        public int SearchResultCount()
        {
            return _searchResults.Count();
        }

        public bool GoToZeroBank(string url)
        {
            _currentDriver.Navigate().GoToUrl(url);
            if (_currentDriver.Url == url)
                return true;
            else
                return false;
        }
        
    }
}

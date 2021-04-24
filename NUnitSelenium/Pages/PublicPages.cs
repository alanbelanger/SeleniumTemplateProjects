using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace NUnitSelenium
{
    internal class PublicPages
    {
        private IWebDriver currentDriver;
        private IWebElement searchBox => currentDriver.FindElement(By.Id("searchTerm"));
        private IList<IWebElement> searchResults => currentDriver.FindElements(By.CssSelector("div.top_offset>ul>li"));

        public PublicPages(IWebDriver driver)
        {
            currentDriver = driver;
        }
        public void SearchForText(string name)
        {
            searchBox.SendKeys(name);
            searchBox.SendKeys(Keys.Enter);
        }
        public int SearchResultCount()
        {
            return searchResults.Count();
        }

        public bool GoToZeroBank(string url)
        {
            currentDriver.Navigate().GoToUrl(url);
            if (currentDriver.Url == url)
                return true;
            else
                return false;
        }
        
    }
}

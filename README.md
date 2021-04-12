# SeleniumTemplateProjects

## Features

This project is a fully-working template that does basic tests on the public site below:
```
http://zero.webappsecurity.com/
```
The test includes some intentional failures to demonstrate the screenshot handling.

WARNING: This target site is intentionally insecure, it is used for pen-testing practice. And sometimes
the authenticated pages may break (b/c people are destroying it on purpose, i guess) which makes
it a decent candidate to run regression tests on. 

Includes:
* NUnit with Selenium and Page Object Model
* Allure report generation
* Snapshot capture on failed assert
* event listener that captures console errors from the browser


## Allure Reports

Install the Allure engine using scoop (get scoop first if you don't have it)
```
    scoop install allure
```

Modify the allureConfig.json to point to your directory to store the allure files.

Generate the allure report using (for example)
```
    allure serve C:\SeleniumTemplateProjects\NUnitSelenium\TestResults
```

## NuGet package notes
To get the event listener to log JS errors the application reports on the browser console, 
you need to install the pre-release (beta) versions of 
    Selenium.Support
    Selenium.WebDriver


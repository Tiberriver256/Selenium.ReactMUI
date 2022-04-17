namespace Selenium.ReactMUI.Test.PageObjects;

using OpenQA.Selenium;

public abstract class PageObject
{

    protected PageObject(IWebDriver webDriver) =>
        this.Driver = webDriver ?? throw new ArgumentNullException(nameof(webDriver));

    protected IWebDriver Driver { get; }

    /// <summary>
    /// Waits for the page to load.
    /// </summary>
    public abstract void WaitForPageToLoad();
}

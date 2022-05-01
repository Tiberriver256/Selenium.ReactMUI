namespace Selenium.ReactMUI.Test.PageObjects;

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

public abstract class PageObject
{
    protected PageObject(IWebDriver webDriver)
    {
        this.Driver = webDriver ?? throw new ArgumentNullException(nameof(webDriver));
        this.Wait = new WebDriverWait(this.Driver, TimeSpan.FromSeconds(10));
    }

    protected static By ParentElement => By.XPath("./..");

    protected IWebDriver Driver { get; }

    protected WebDriverWait Wait { get; }

    /// <summary>
    /// Waits for the page to load.
    /// </summary>
    public abstract void WaitForPageToLoad();
}

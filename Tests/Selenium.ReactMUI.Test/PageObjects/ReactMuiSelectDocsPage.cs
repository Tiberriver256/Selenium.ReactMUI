namespace Selenium.ReactMUI.Test.PageObjects;

using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

public class ReactMuiSelectDocsPage : PageObject
{
    public ReactMuiSelectDocsPage(IWebDriver driver)
        : base(driver) => PageFactory.InitElements<ReactMuiSelectDocsPage>(driver);

    [FindsBy(How = How.TagName, Using = "h1")]
    public IWebElement Header { get; set; }

    public override void WaitForPageToLoad() => _ = this.Header.Text;
}

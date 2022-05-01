namespace Selenium.ReactMUI.Test.PageObjects;

using OpenQA.Selenium;
using Selenium.ReactMUI.Components;
using SeleniumExtras.WaitHelpers;

public class ReactMuiSelectDocsPage : PageObject
{
    private readonly By demoSimpleSelect = By.Id("demo-simple-select");
    private readonly Uri uri = new("https://mui.com/material-ui/react-select/#basic-select");

    public ReactMuiSelectDocsPage(IWebDriver driver)
        : base(driver)
    {
    }

    public IWebElement Header => this.Wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("h1")));

    public MuiSelectElement BasicSelect
    {
        get
        {
            var select = this.Wait.Until(ExpectedConditions.ElementToBeClickable(this.demoSimpleSelect));
            return new MuiSelectElement(select.FindElement(ParentElement), this.Driver);
        }
    }

    public void NavigateToPage()
    {
        this.Driver.Navigate().GoToUrl(this.uri);
        this.WaitForPageToLoad();
    }

    public override void WaitForPageToLoad() => _ = this.Header.Text;
}

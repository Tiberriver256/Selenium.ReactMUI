namespace Selenium.ReactMUI.Components.Tests;

using FluentAssertions;
using OpenQA.Selenium;
using Selenium.ReactMUI.Test.PageObjects;
using Xunit;

public class MuiSelectElementTests
{
    private readonly ReactMuiSelectDocsPage reactMuiSelectDocsPage;

    public MuiSelectElementTests(IWebDriver webDriver)
    {
        this.reactMuiSelectDocsPage = new ReactMuiSelectDocsPage(webDriver);
        this.reactMuiSelectDocsPage.NavigateToPage();
    }

    [Fact]
    public void MuiSelectElementTest()
    {
        // Arrange
        this.reactMuiSelectDocsPage.WaitForPageToLoad();

        // Assert
        this.reactMuiSelectDocsPage.BasicSelect.SelectByText("Ten");

        // Act
        this.reactMuiSelectDocsPage.BasicSelect.SelectedOption.Text.Should().Be("Ten");
    }
}

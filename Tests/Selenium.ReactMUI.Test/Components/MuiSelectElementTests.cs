namespace Selenium.ReactMUI.Components.Tests;

using Selenium.ReactMUI.Test.BaseClasses;
using Selenium.ReactMUI.Test.Fixtures;
using Xunit;

public class MuiSelectElementTests : DocsPageTest, IClassFixture<WebDriverFixture>
{
    public MuiSelectElementTests(WebDriverFixture webdriverFixture) =>
        this.WebDriverFixture = webdriverFixture ?? throw new ArgumentNullException(nameof(webdriverFixture));

    internal override Uri DocsPage { get; } = new Uri("https://mui.com/material-ui/react-select/#basic-select");

    internal override WebDriverFixture WebDriverFixture { get; }

    [Fact]
    public void MuiSelectElementTest()
    {
        // Arrange
        this.NavigateToDocsPage();

        // Assert
        // Act
    }

    [Fact]
    public void SelectByTextTest()
    {
        Assert.True(false, "This test needs an implementation");
    }

    [Fact]
    public void SelectByValueTest()
    {
        Assert.True(false, "This test needs an implementation");
    }

    [Fact]
    public void SelectByIndexTest()
    {
        Assert.True(false, "This test needs an implementation");
    }

    [Fact]
    public void DeselectAllTest()
    {
        Assert.True(false, "This test needs an implementation");
    }

    [Fact]
    public void DeselectByTextTest()
    {
        Assert.True(false, "This test needs an implementation");
    }

    [Fact]
    public void DeselectByValueTest()
    {
        Assert.True(false, "This test needs an implementation");
    }

    [Fact]
    public void DeselectByIndexTest()
    {
        Assert.True(false, "This test needs an implementation");
    }
}

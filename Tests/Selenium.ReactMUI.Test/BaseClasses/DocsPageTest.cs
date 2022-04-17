namespace Selenium.ReactMUI.Test.BaseClasses;
using System;
using Selenium.ReactMUI.Test.Fixtures;

public abstract class DocsPageTest
{
    internal abstract Uri DocsPage { get; }

    internal abstract WebDriverFixture WebDriverFixture { get; }

    internal void NavigateToDocsPage() => this.WebDriverFixture.Driver.Navigate().GoToUrl(this.DocsPage);
}

namespace Selenium.ReactMUI.Components;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using OpenQA.Selenium;

/// <summary>
/// Wraps Select
/// https://mui.com/material-ui/react-select/.
/// </summary>
public class MuiSelectElement : IWrapsElement
{
    private readonly IWebDriver driver;

    /// <summary>
    /// Initializes a new instance of the <see cref="MuiSelectElement"/> class.
    /// </summary>
    /// <param name="element">The select web element.</param>
    /// <param name="driver">An instance of <see cref="IWebDriver"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown when element or driver arguments are null.</exception>
    /// <exception cref="ArgumentException">Thrown when the <see cref="IWebElement"/> is NativeSelect or a select element.</exception>
    public MuiSelectElement(IWebElement element, IWebDriver driver)
    {
        this.WrappedElement = element ?? throw new ArgumentNullException(nameof(element), $"{nameof(element)} cannot be null");
        this.driver = driver ?? throw new ArgumentNullException(nameof(driver), $"{nameof(driver)} cannot be null");
        var isNative = element.FindElements(By.CssSelector("select")).Any() || element.TagName.Equals("select", StringComparison.OrdinalIgnoreCase);

        if (isNative)
        {
            throw new ArgumentException("Please use Selenium.Support.SelectElement for working with NativeSelect", nameof(element));
        }

        this.IsMultiple = element.FindElements(By.CssSelector(".MuiSelect-multiple")).Any();
    }

    /// <summary>
    /// Gets the <see cref="IWebElement"/> wrapped by this object.
    /// </summary>
    public IWebElement WrappedElement { get; }

    /// <summary>
    /// Gets a value indicating whether the parent element supports multiple selections.
    /// </summary>
    public bool IsMultiple
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the list of options for the select element.
    /// </summary>
    public IList<IWebElement> Options
    {
        get
        {
            this.OpenMenu();

            return this.driver.FindElements(By.CssSelector("[role=option]"));
        }
    }

    /// <summary>
    /// Gets all disabled options.
    /// </summary>
    public IList<IWebElement> AllDisabledOptions
    {
        get
        {
            var list = new List<IWebElement>();
            foreach (var option in this.Options)
            {
                if (option.GetAttribute("aria-disabled") == "true")
                {
                    list.Add(option);
                }
            }

            return list;
        }
    }

    /// <summary>
    /// Gets the selected item within the select element.
    /// </summary>
    /// <remarks>If more than one item is selected this will return the first item.</remarks>
    /// <exception cref="NoSuchElementException">Thrown if no option is selected.</exception>
    public IWebElement SelectedOption
    {
        get
        {
            foreach (var option in this.Options)
            {
                if (option.GetAttribute("aria-selected") == "true")
                {
                    return option;
                }
            }

            throw new NoSuchElementException("No option is selected");
        }
    }

    /// <summary>
    /// Gets all of the selected options within the select element.
    /// </summary>
    public IList<IWebElement> AllSelectedOptions
    {
        get
        {
            var list = new List<IWebElement>();
            foreach (var option in this.Options)
            {
                if (option.GetAttribute("aria-selected") == "true")
                {
                    list.Add(option);
                }
            }

            return list;
        }
    }

    private bool MenuIsOpen => this.WrappedElement.FindElements(By.CssSelector("[role=button][aria-expanded=true]")).Any();

    /// <summary>
    /// Select all options by the text displayed.
    /// </summary>
    /// <param name="text">The text of the option to be selected.</param>
    /// <param name="partialMatch">Default value is false. If true a partial match on the Options list will be performed, otherwise exact match.</param>
    /// <remarks>When given "Bar" this method would select an option like:
    /// <para>
    /// &lt;MenuItem value="foo"&gt;Bar&lt;/MenuItem&gt;
    /// </para>
    /// </remarks>
    /// <exception cref="NoSuchElementException">Thrown if there is no element with the given text present.</exception>
    public void SelectByText(string text, bool partialMatch = false)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text), "text must not be null");
        }

        var flag = false;
        this.OpenMenu();

        var optionsElement = this.driver.FindElement(By.CssSelector(".MuiMenu-root[role=presentation]"));
        IList<IWebElement> list = partialMatch ? optionsElement.FindElements(By.XPath(".//*[@role = \"option\" and contains(normalize-space(.),  " + EscapeQuotes(text) + ")]")) : optionsElement.FindElements(By.XPath(".//*[@role = \"option\" and normalize-space(.) = " + EscapeQuotes(text) + "]"));
        foreach (var item in list)
        {
            this.SetSelected(item, select: true);
            if (!this.IsMultiple)
            {
                return;
            }

            flag = true;
        }

        if (list.Count == 0 && text.Contains(' ', StringComparison.OrdinalIgnoreCase))
        {
            var longestSubstringWithoutSpace = GetLongestSubstringWithoutSpace(text);
            IList<IWebElement> list2 = (!string.IsNullOrEmpty(longestSubstringWithoutSpace)) ? optionsElement.FindElements(By.XPath(".//*[@role = \"option\" and contains(., " + EscapeQuotes(longestSubstringWithoutSpace) + ")]")) : optionsElement.FindElements(By.CssSelector("[role=option]"));
            foreach (var item2 in list2)
            {
                if (text == item2.Text)
                {
                    this.SetSelected(item2, select: true);
                    if (!this.IsMultiple)
                    {
                        return;
                    }

                    flag = true;
                }
            }
        }

        if (!flag)
        {
            throw new NoSuchElementException("Cannot locate element with text: " + text);
        }
    }

    /// <summary>
    /// Select an option by the value.
    /// </summary>
    /// <param name="value">The value of the option to be selected.</param>
    /// <remarks>When given "foo" this method will select an option like:
    /// <para>
    /// &lt;MenuItem value="foo"&gt;Bar&lt;/MenuItem&gt;
    /// </para>
    /// </remarks>
    /// <remarks>Will only work with primitive values.</remarks>
    /// <exception cref="ArgumentNullException">Thrown when <see cref="string"/> is null.</exception>
    /// <exception cref="NoSuchElementException">Thrown when no element with the specified value is found.</exception>
    public void SelectByValue(string value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var stringBuilder = new StringBuilder(".//*[@role = \"option\" and @data-value = ");
        stringBuilder.Append(EscapeQuotes(value));
        stringBuilder.Append(']');

        this.OpenMenu();

        var optionsElement = this.driver.FindElement(By.CssSelector(".MuiMenu-root[role=presentation]"));
        IList<IWebElement> list = optionsElement.FindElements(By.XPath(stringBuilder.ToString()));
        var flag = false;
        foreach (var item in list)
        {
            this.SetSelected(item, select: true);
            if (!this.IsMultiple)
            {
                return;
            }

            flag = true;
        }

        if (!flag)
        {
            throw new NoSuchElementException("Cannot locate option with value: " + value);
        }
    }

    /// <summary>
    /// Select the option by the index, as determined by the placment of the option in the list of options.
    /// </summary>
    /// <param name="index">The value of the index attribute of the option to be selected.</param>
    /// <exception cref="NoSuchElementException">Thrown when no element exists at the specified index.</exception>
    /// <exception cref="IndexOutOfRangeException">Thrown when the index provided is greater than the available options.</exception>
    public void SelectByIndex(int index) => this.SetSelected(this.Options[index], select: true);

    /// <summary>
    /// Clear all selected entries. This is only valid when the SELECT supports multiple selections.
    /// </summary>
    /// <exception cref="WebDriverException">Thrown when attempting to deselect all options from a SELECT
    /// that does not support multiple selections.</exception>
    public void DeselectAll()
    {
        if (!this.IsMultiple)
        {
            throw new InvalidOperationException("You may only deselect all options if multi-select is supported");
        }

        foreach (var option in this.Options)
        {
            this.SetSelected(option, select: false);
        }
    }

    /// <summary>
    /// Deselect the option by the text displayed.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when attempting to deselect option from a SELECT
    /// that does not support multiple selections.</exception>
    /// <exception cref="NoSuchElementException">Thrown when no element exists with the specified test attribute.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <see cref="string"/> is null.</exception>
    /// <param name="text">The text of the option to be deselected.</param>
    /// <remarks>When given "Bar" this method would deselect an option like:
    /// <para>
    /// &lt;MenuItem value="foo"&gt;Bar&lt;/MenuItem&gt;
    /// </para>
    /// </remarks>
    public void DeselectByText(string text)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        if (!this.IsMultiple)
        {
            throw new InvalidOperationException("You may only deselect option if multi-select is supported");
        }

        var flag = false;
        var stringBuilder = new StringBuilder(".//*[@role = \"option\" and normalize-space(.) = ");
        stringBuilder.Append(EscapeQuotes(text));
        stringBuilder.Append(']');

        this.OpenMenu();

        var optionsElement = this.driver.FindElement(By.CssSelector(".MuiMenu-root[role=presentation]"));
        IList<IWebElement> list = optionsElement.FindElements(By.XPath(stringBuilder.ToString()));
        foreach (var item in list)
        {
            this.SetSelected(item, select: false);
            flag = true;
        }

        if (!flag)
        {
            throw new NoSuchElementException("Cannot locate option with text: " + text);
        }
    }

    /// <summary>
    /// Deselect the option having value matching the specified text.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when attempting to deselect option from a SELECT
    /// that does not support multiple selections.</exception>
    /// <exception cref="NoSuchElementException">Thrown when no element exists with the specified value attribute.</exception>
    /// <param name="value">The value of the option to deselect.</param>
    /// <remarks>When given "foo" this method will deselect an option like:
    /// <para>
    /// &lt;MenuItem value="foo"&gt;Bar&lt;/MenuItem&gt;
    /// </para>
    /// </remarks>
    public void DeselectByValue(string value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (!this.IsMultiple)
        {
            throw new InvalidOperationException("You may only deselect option if multi-select is supported");
        }

        var flag = false;
        var stringBuilder = new StringBuilder(".//*[@role = \"option\" and @data-value = ");
        stringBuilder.Append(EscapeQuotes(value));
        stringBuilder.Append(']');

        this.OpenMenu();

        var optionsElement = this.driver.FindElement(By.CssSelector(".MuiMenu-root[role=presentation]"));
        IList<IWebElement> list = optionsElement.FindElements(By.XPath(stringBuilder.ToString()));
        foreach (var item in list)
        {
            this.SetSelected(item, select: false);
            flag = true;
        }

        if (!flag)
        {
            throw new NoSuchElementException("Cannot locate option with value: " + value);
        }
    }

    /// <summary>
    /// Deselect the option by the index, as determined by the placement of the option inside the select.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when attempting to deselect option from a SELECT
    /// that does not support multiple selections.</exception>
    /// <param name="index">The index of the option to deselect.</param>
    public void DeselectByIndex(int index)
    {
        if (!this.IsMultiple)
        {
            throw new InvalidOperationException("You may only deselect option if multi-select is supported");
        }

        this.SetSelected(this.Options[index], select: false);
    }

    private static string EscapeQuotes(string toEscape)
    {
        if (toEscape.IndexOf("\"", StringComparison.OrdinalIgnoreCase) > -1 && toEscape.IndexOf("'", StringComparison.OrdinalIgnoreCase) > -1)
        {
            var flag = false;
            if (toEscape.LastIndexOf("\"", StringComparison.OrdinalIgnoreCase) == toEscape.Length - 1)
            {
                flag = true;
            }

            var list = new List<string>(toEscape.Split('"'));
            if (flag && string.IsNullOrEmpty(list[^1]))
            {
                list.RemoveAt(list.Count - 1);
            }

            var stringBuilder = new StringBuilder("concat(");
            for (var i = 0; i < list.Count; i++)
            {
                stringBuilder.Append('"').Append(list[i]).Append('"');
                if (i == list.Count - 1)
                {
                    if (flag)
                    {
                        stringBuilder.Append(", '\"')");
                    }
                    else
                    {
                        stringBuilder.Append(')');
                    }
                }
                else
                {
                    stringBuilder.Append(", '\"', ");
                }
            }

            return stringBuilder.ToString();
        }

        if (toEscape.IndexOf("\"", StringComparison.OrdinalIgnoreCase) > -1)
        {
            return string.Format(CultureInfo.InvariantCulture, "'{0}'", toEscape);
        }

        return string.Format(CultureInfo.InvariantCulture, "\"{0}\"", toEscape);
    }

    private static string GetLongestSubstringWithoutSpace(string s)
    {
        var text = string.Empty;
        var array = s.Split(' ');
        var array2 = array;
        foreach (var text2 in array2)
        {
            if (text2.Length > text.Length)
            {
                text = text2;
            }
        }

        return text;
    }

    private void OpenMenu()
    {
        if (!this.MenuIsOpen)
        {
            this.WrappedElement.Click();
        }
    }

    private void CloseMenu()
    {
        var backdrop = this.driver.FindElement(By.CssSelector(".MuiMenu-root[role=presentation]"));
        backdrop.Click();
    }

    private void SetSelected(IWebElement option, bool select)
    {
        var selected = option.GetAttribute("aria-selected") == "true";
        if ((!selected && select) || (selected && !select))
        {
            option.Click();
        }
        else
        {
            this.CloseMenu();
        }
    }
}

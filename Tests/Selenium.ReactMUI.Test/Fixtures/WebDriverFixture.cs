namespace Selenium.ReactMUI.Test.Fixtures;
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

public class WebDriverFixture : IDisposable
{
    private readonly ChromeDriverService chromeDriverService;
    private bool disposedValue;

    public WebDriverFixture()
    {
        var driverPath = Environment.CurrentDirectory;
        var envChromeWebDriver = Environment.GetEnvironmentVariable("ChromeWebDriver");
        if (!string.IsNullOrEmpty(envChromeWebDriver) &&
            File.Exists(Path.Combine(envChromeWebDriver, "chromedriver.exe")))
        {
            driverPath = envChromeWebDriver;
        }

        this.chromeDriverService = ChromeDriverService.CreateDefaultService(driverPath);

        if (this.chromeDriverService.HostName.Equals("localhost", StringComparison.OrdinalIgnoreCase))
        {
            this.chromeDriverService.HostName = "127.0.0.1";
        }

        var options = new ChromeOptions();
        options.AddArguments("--headless", "--disable-gpu");

        this.Driver = new ChromeDriver(this.chromeDriverService, options);
        this.Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
    }

    public IWebDriver Driver { get; private set; }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                this.Driver.Dispose();
                this.chromeDriverService?.Dispose();
            }

            this.disposedValue = true;
        }
    }
}

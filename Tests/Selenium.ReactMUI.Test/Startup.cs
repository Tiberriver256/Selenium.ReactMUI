namespace Selenium.ReactMUI.Test;
using System;
using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

public static class Startup
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(sp =>
        {
            var driverPath = Environment.CurrentDirectory;
            var envChromeWebDriver = Environment.GetEnvironmentVariable("ChromeWebDriver");
            if (!string.IsNullOrEmpty(envChromeWebDriver) &&
                File.Exists(Path.Combine(envChromeWebDriver, "chromedriver.exe")))
            {
                driverPath = envChromeWebDriver;
            }

            var chromeDriverService = ChromeDriverService.CreateDefaultService(driverPath);

            if (chromeDriverService.HostName.Equals("localhost", StringComparison.OrdinalIgnoreCase))
            {
                chromeDriverService.HostName = "127.0.0.1";
            }

            return chromeDriverService;
        });

        services.AddTransient<IWebDriver, ChromeDriver>(sp =>
        {
            var chromeDriverService = sp.GetRequiredService<ChromeDriverService>();

            var options = new ChromeOptions();
            //options.AddArguments("--headless", "--disable-gpu");

            var driver = new ChromeDriver(chromeDriverService, options);
            return driver;
        });
    }
}

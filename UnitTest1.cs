using Xunit;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace SimpleCalculatorWeblinux.UITests
{
    public class CalculatorUITestsFixture : IDisposable
    {
        public IWebDriver Driver { get; private set; }
        public string AppUrl { get; private set; }

        public CalculatorUITestsFixture()
        {
            Driver = new ChromeDriver();
            AppUrl = GetAppUrl();
        }

        public void Dispose()
        {
            Driver?.Quit();
        }

        private string GetAppUrl()
        {
            var jsonPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "appsettings.json");
            var json = File.ReadAllText(jsonPath);
            var jObject = JObject.Parse(json);
            return jObject["AppSettings"]["appUrl"].ToString();
        }
    }

    [CollectionDefinition("Calculator Test Collection")]
    public class CalculatorTestCollection : ICollectionFixture<CalculatorUITestsFixture>
    {
    }

    [Collection("Calculator Test Collection")]
    public class CalculatorUITests
    {
        private readonly CalculatorUITestsFixture _fixture;

        public CalculatorUITests(CalculatorUITestsFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void Add_TwoNumbers_UITest()
        {
            PerformOperation("5", "3", "Add", "8");
        }

        [Fact]
        public void Subtract_TwoNumbers_UITest()
        {
            PerformOperation("5", "3", "Subtract", "2");
        }

        [Fact]
        public void Multiply_TwoNumbers_UITest()
        {
            PerformOperation("5", "3", "Multiply", "15");
        }

        [Fact]
        public void Divide_TwoNumbers_UITest()
        {
            PerformOperation("6", "3", "Divide", "2");
        }

        private void PerformOperation(string valueA, string valueB, string operation, string expectedResult)
        {
            _fixture.Driver.Navigate().GoToUrl(_fixture.AppUrl);

            _fixture.Driver.FindElement(By.Id("ValueA")).SendKeys(valueA);
            _fixture.Driver.FindElement(By.Id("ValueB")).SendKeys(valueB);
            _fixture.Driver.FindElement(By.CssSelector($"button[value='{operation}']")).Click();

            var wait = new WebDriverWait(_fixture.Driver, TimeSpan.FromSeconds(10));
            var resultElement = wait.Until(drv =>
            {
                var element = drv.FindElement(By.Id("Result"));
                if (element.GetAttribute("value") != "")
                {
                    return element;
                }
                return null;
            });

            var result = resultElement?.GetAttribute("value");
            Assert.Equal(expectedResult, result);
        }
    }
}

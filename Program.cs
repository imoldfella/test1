
using Boa.Constrictor.Screenplay;
using Boa.Constrictor.Logging;
using Boa.Constrictor.WebDriver;
using static Boa.Constrictor.WebDriver.WebLocator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using FluentAssertions;

[TestClass]
public class UnitTest1
{
    static TestContext? context;

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
      context = testContext;
    }

    [TestMethod]
    public void test1()
    {
        var d = new ChromeDriver();
        var ts = (ITakesScreenshot)d;
        var actor = new Actor(name: "andy", logger: new ConsoleLogger());
        try
        {
            actor.Can(BrowseTheWeb.With(d));
            actor.AttemptsTo(Navigate.ToUrl(SearchPage.Url));
            // Get the page's title
            string title = actor.AsksFor(Title.OfPage());

            // Search for something
            actor.AttemptsTo(SearchDuckDuckGo.For("panda"));

            // Wait for results
            actor.WaitsUntil(Appearance.Of(ResultPage.ResultLinks), IsEqualTo.True());
        } catch {
          var ss = ts.GetScreenshot();
          ss.SaveAsFile("test1_fail");
          context?.AddResultFile("test1_fail");
          throw;
        }
        finally
        {
            actor.AttemptsTo(QuitWebDriver.ForBrowser());
        }
    }
}

public static class SearchPage
{
    public const string Url = "https://www.duckduckgo.com/";

    public static IWebLocator SearchInput => L(
      "DuckDuckGo Search Input",
      By.Id("search_form_input_homepage"));

    public static IWebLocator SearchButton => L(
      "DuckDuckGo Search Button",
      By.Id("search_button_homepage"));
    // By id is shorthand for:
    //By.CssSelector("#search_button_homepage")
    // Good practice for your DOM queries.
}

public static class ResultPage
{
    public static IWebLocator ResultLinks => L(
      "DuckDuckGo Result Page Links",
      By.ClassName("result__a"));
}


public class SearchDuckDuckGo : ITask
{
    public string Phrase { get; }

    private SearchDuckDuckGo(string phrase) =>
      Phrase = phrase;

    public static SearchDuckDuckGo For(string phrase) =>
      new SearchDuckDuckGo(phrase);

    public void PerformAs(IActor actor)
    {
        actor.AttemptsTo(SendKeys.To(SearchPage.SearchInput, Phrase));
        actor.AttemptsTo(Click.On(SearchPage.SearchButton));
    }
}

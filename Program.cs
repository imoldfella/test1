
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
        var actor = new ChromeActor(context!);
        try
        {
            actor.AttemptsTo(Navigate.ToUrl(SearchPage.Url));
            // Get the page's title
            string title = actor.AsksFor(Title.OfPage());
            actor.screenshot("test1_start");
            // Search for something
            actor.AttemptsTo(SearchDuckDuckGo.For("panda"));

            // Wait for results
            actor.WaitsUntil(Appearance.Of(ResultPage.ResultLinks), IsEqualTo.True());
        } catch {
          actor.screenshot("test1_fail");
          throw;
        }
        finally
        {
            actor.AttemptsTo(QuitWebDriver.ForBrowser());
        }
    }
}

// abstract some things into an extended actor
public class ChromeActor : Actor {
  public TestContext context;
  public ITakesScreenshot ts;
  public ChromeActor(TestContext context){
        this.context = context;
        var opt = new ChromeOptions();
        opt.AddArgument("headless");
        opt.AddArgument("window-size=1920,1200");
        opt.AddArgument("start-maximized");

        var d = new ChromeDriver(opt);
        this.ts = (ITakesScreenshot)d;
        var actor = new Actor(name: "andy", logger: new ConsoleLogger());  
        this.Can(BrowseTheWeb.With(d));
  }
  public void screenshot(string file) {
          var ss = ts.GetScreenshot();
          ss.SaveAsFile(file+".jpg");
          context?.AddResultFile(file+".jpg");
  }

}

public class SearchPage
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

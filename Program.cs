
using Boa.Constrictor.Screenplay;
using Boa.Constrictor.Logging;
using Boa.Constrictor.WebDriver;
using static Boa.Constrictor.WebDriver.WebLocator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using FluentAssertions;
//using NUnit.Framework;

[TestClass]
public class UnitTest1 {

    // public static void Main(){
    //     (new UnitTest1()).test1();
    // }
    [TestMethod]
    public void test1()
    {
        var actor = new Actor(name: "andy", logger: new ConsoleLogger());
        actor.Can(BrowseTheWeb.With(new ChromeDriver()));
        actor.AttemptsTo(Navigate.ToUrl(SearchPage.Url));
        // Get the page's title
        string title = actor.AsksFor(Title.OfPage());

        // Search for something
        actor.AttemptsTo(SearchDuckDuckGo.For("panda"));

        // Wait for results
        actor.WaitsUntil(Appearance.Of(ResultPage.ResultLinks), IsEqualTo.True());

        actor.AttemptsTo(QuitWebDriver.ForBrowser());
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

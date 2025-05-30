using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;

namespace WanderlustJournal.Tests;

[TestClass]
public class HomePageTests : PageTest
{
    [TestMethod]
    public async Task HomepageShouldRender()
    {
        await Page.GotoAsync(PlaywrightFixture.BaseUrl);
        
        // Verify the page title
        await Expect(Page).ToHaveTitleAsync("Welcome to Wanderlust Journal - WanderlustJournal");
        
        // Verify that the main heading is present
        var heading = Page.GetByRole(AriaRole.Heading, new() { Name = "Wanderlust Journal" });
        await Expect(heading).ToBeVisibleAsync();
        
        // Verify the action buttons exist
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "View All Entries" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Add New Memory" })).ToBeVisibleAsync();
    }
}
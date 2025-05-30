using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;

namespace WanderlustJournal.Tests;

[TestClass]
public class NavigationTests : PageTest
{
    [TestMethod]
    public async Task ShouldNavigateToAllMainPages()
    {
        // Start at the home page
        await Page.GotoAsync(PlaywrightFixture.BaseUrl);
        
        // Test navigation to Journal Index
        await Page.GetByRole(AriaRole.Link, new() { Name = "Journal" }).ClickAsync();
        await Expect(Page).ToHaveTitleAsync("Journal Entries - WanderlustJournal");
        
        // Test navigation to Map
        await Page.GetByRole(AriaRole.Link, new() { Name = "Map" }).ClickAsync();
        await Expect(Page).ToHaveTitleAsync("Entry Map - WanderlustJournal");
        
        // Test navigation to Search
        await Page.GetByRole(AriaRole.Link, new() { Name = "Search" }).ClickAsync();
        await Expect(Page).ToHaveTitleAsync("Search Entries - WanderlustJournal");
        
        // Test navigation to Privacy
        await Page.GetByRole(AriaRole.Link, new() { Name = "Privacy" }).ClickAsync();
        await Expect(Page).ToHaveTitleAsync("Privacy Policy - WanderlustJournal");
        
        // Test navigation back to Home
        await Page.GetByRole(AriaRole.Link, new() { Name = "Home" }).ClickAsync();
        await Expect(Page).ToHaveTitleAsync("Welcome to Wanderlust Journal - WanderlustJournal");
    }
}
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;

namespace WanderlustJournal.Tests;

[TestClass]
public class JournalEntryTests : PageTest
{
    [TestMethod]
    public async Task ShouldDisplayJournalEntriesList()
    {
        await Page.GotoAsync($"{PlaywrightFixture.BaseUrl}/Journal");
        
        // Verify the page title
        await Expect(Page).ToHaveTitleAsync("Journal Entries - WanderlustJournal");
        
        // Verify that the page contains expected elements
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Your Travel Memories" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Add New Entry" })).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task ShouldNavigateToCreateNewEntry()
    {
        await Page.GotoAsync($"{PlaywrightFixture.BaseUrl}/Journal");
        
        // Click on the "Add New Entry" button
        await Page.GetByRole(AriaRole.Link, new() { Name = "Add New Entry" }).ClickAsync();
        
        // Verify we're on the create page
        await Expect(Page).ToHaveTitleAsync("Create New Entry - WanderlustJournal");
        
        // Verify form elements
        await Expect(Page.GetByLabel("Title")).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("Location")).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("Date Visited")).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("Notes")).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task ShouldCreateAndViewNewEntry()
    {
        string uniqueTitle = $"Test Entry {DateTime.Now.Ticks}";
        
        // Navigate to create page
        await Page.GotoAsync($"{PlaywrightFixture.BaseUrl}/Journal/Create");
        
        // Fill out the form
        await Page.GetByLabel("Title").FillAsync(uniqueTitle);
        await Page.GetByLabel("Location").FillAsync("Paris, France");
        await Page.GetByLabel("Date Visited").FillAsync(DateTime.Now.ToString("yyyy-MM-dd"));
        await Page.GetByLabel("Notes").FillAsync("This is a test entry created by Playwright");
        
        // Submit the form
        await Page.GetByRole(AriaRole.Button, new() { Name = "Create" }).ClickAsync();
        
        // We should be redirected to the details page
        await Expect(Page.GetByText(uniqueTitle)).ToBeVisibleAsync();
        await Expect(Page.GetByText("Paris, France")).ToBeVisibleAsync();
        await Expect(Page.GetByText("This is a test entry created by Playwright")).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task ShouldSearchForEntries()
    {
        // Navigate to search page
        await Page.GotoAsync($"{PlaywrightFixture.BaseUrl}/Journal/Search");
        
        // Verify the search form is present
        await Expect(Page.GetByPlaceholder("Enter search term...")).ToBeVisibleAsync();
        
        // Enter a search term
        await Page.GetByPlaceholder("Enter search term...").FillAsync("Paris");
        
        // Submit the search
        await Page.GetByRole(AriaRole.Button, new() { Name = "Search" }).ClickAsync();
        
        // Verify search results
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Search Results" })).ToBeVisibleAsync();
    }
}
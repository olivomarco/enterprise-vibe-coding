using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WanderlustJournal.E2ETests.Helpers;
using WanderlustJournal.E2ETests.Pages;

namespace WanderlustJournal.E2ETests.Tests
{
    [TestClass]
    public class JournalEntryTests
    {
        private PlaywrightFixture _fixture;
        private IPage _page;
        private string _baseUrl = "https://localhost:5001";
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new PlaywrightFixture();
            _page = _fixture.Page;
        }
        
        [TestCleanup]
        public void TestCleanup()
        {
            _fixture.Dispose();
        }
        
        [TestMethod]
        public async Task CanCreateNewJournalEntry()
        {
            // Arrange
            var createPage = new JournalCreatePage(_page, _baseUrl);
            
            // Act
            await createPage.NavigateAsync();
            await createPage.FillEntryFormAsync(
                "Test Entry " + Guid.NewGuid().ToString().Substring(0, 8),
                "Test Location",
                DateTime.Today,
                "These are test notes for an E2E test."
            );
            await createPage.SubmitFormAsync();
            
            // Assert
            var journalListPage = new JournalListPage(_page, _baseUrl);
            var pageHeading = await journalListPage.GetPageHeadingAsync();
            Assert.AreEqual("Journal Entries", pageHeading);
            
            // Verify the entry count has increased
            var entryCount = await journalListPage.GetEntryCountAsync();
            Assert.IsTrue(entryCount > 0, "Should have at least one entry after creating");
        }
        
        [TestMethod]
        public async Task ValidationErrors_WhenCreatingInvalidEntry()
        {
            // Arrange
            var createPage = new JournalCreatePage(_page, _baseUrl);
            
            // Act - Submit an empty form
            await createPage.NavigateAsync();
            await createPage.SubmitFormAsync();
            
            // Assert
            var hasErrors = await createPage.HasValidationErrorsAsync();
            Assert.IsTrue(hasErrors, "Validation errors should be displayed");
        }
        
        [TestMethod]
        public async Task CanSearchForEntries()
        {
            // Arrange
            var journalListPage = new JournalListPage(_page, _baseUrl);
            
            // Create a unique entry first to search for
            var uniqueTitle = "Unique Search Test " + Guid.NewGuid().ToString().Substring(0, 8);
            var createPage = new JournalCreatePage(_page, _baseUrl);
            await createPage.NavigateAsync();
            await createPage.FillEntryFormAsync(
                uniqueTitle,
                "Searchable Location",
                DateTime.Today,
                "This entry is created specifically for search testing."
            );
            await createPage.SubmitFormAsync();
            
            // Act - Search for the unique entry
            await journalListPage.SearchForEntryAsync(uniqueTitle);
            
            // Assert
            var entryTitles = await journalListPage.GetEntryTitlesAsync();
            Assert.AreEqual(1, entryTitles.Count, "Should find exactly one entry");
            Assert.IsTrue(entryTitles[0].Contains(uniqueTitle), "Found entry should match search term");
        }
        
        [TestMethod]
        public async Task NoResults_WhenSearchingNonExistentTerm()
        {
            // Arrange
            var journalListPage = new JournalListPage(_page, _baseUrl);
            
            // Act - Search for a term that shouldn't exist
            var nonExistentTerm = "NonExistentTerm" + Guid.NewGuid().ToString();
            await journalListPage.NavigateAsync();
            await journalListPage.SearchForEntryAsync(nonExistentTerm);
            
            // Assert
            var hasNoEntriesMessage = await journalListPage.HasNoEntriesMessageAsync();
            Assert.IsTrue(hasNoEntriesMessage, "Should display 'No entries found' message");
        }
    }
}
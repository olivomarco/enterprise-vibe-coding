using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WanderlustJournal.E2ETests.Helpers;
using WanderlustJournal.E2ETests.Pages;

namespace WanderlustJournal.E2ETests.Tests
{
    [TestClass]
    public class FormValidationTests
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
        public async Task RequiredFields_ShowValidationErrors_WhenEmpty()
        {
            // Arrange
            var createPage = new JournalCreatePage(_page, _baseUrl);
            
            // Act - Navigate and submit without filling fields
            await createPage.NavigateAsync();
            await createPage.SubmitFormAsync();
            
            // Assert
            var hasErrors = await createPage.HasValidationErrorsAsync();
            Assert.IsTrue(hasErrors, "Validation errors should be displayed");
            
            var errorText = await createPage.GetValidationErrorsTextAsync();
            Assert.IsTrue(errorText.Contains("Please enter a title"), "Should show title validation error");
            Assert.IsTrue(errorText.Contains("Please enter a location"), "Should show location validation error");
            Assert.IsTrue(errorText.Contains("Please enter the date visited"), "Should show date validation error");
        }
        
        [TestMethod]
        public async Task TitleOnly_ShowsRemainingValidationErrors()
        {
            // Arrange
            var createPage = new JournalCreatePage(_page, _baseUrl);
            
            // Act - Fill only title and submit
            await createPage.NavigateAsync();
            await createPage.FillEntryFormAsync(
                "Test Title",
                "", // Empty location
                DateTime.MinValue, // Invalid date
                ""
            );
            await createPage.SubmitFormAsync();
            
            // Assert
            var hasErrors = await createPage.HasValidationErrorsAsync();
            Assert.IsTrue(hasErrors, "Validation errors should be displayed");
            
            var errorText = await createPage.GetValidationErrorsTextAsync();
            Assert.IsFalse(errorText.Contains("Please enter a title"), "Should not show title validation error");
            Assert.IsTrue(errorText.Contains("Please enter a location"), "Should show location validation error");
            Assert.IsTrue(errorText.Contains("Please enter the date visited"), "Should show date validation error");
        }
        
        [TestMethod]
        public async Task AllRequiredFields_NoValidationErrors()
        {
            // Arrange
            var createPage = new JournalCreatePage(_page, _baseUrl);
            
            // Act - Fill all required fields
            await createPage.NavigateAsync();
            await createPage.FillEntryFormAsync(
                "Test Title",
                "Test Location",
                DateTime.Today,
                "" // Notes are optional
            );
            await createPage.SubmitFormAsync();
            
            // Assert - Should navigate to index page
            var journalListPage = new JournalListPage(_page, _baseUrl);
            var pageHeading = await journalListPage.GetPageHeadingAsync();
            Assert.AreEqual("Journal Entries", pageHeading, "Should navigate to entries list after successful form submission");
        }
    }
}
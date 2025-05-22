using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WanderlustJournal.E2ETests.Helpers;
using WanderlustJournal.E2ETests.Pages;

namespace WanderlustJournal.E2ETests.Tests
{
    [TestClass]
    public class NavigationTests
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
        public async Task HomePage_HasCorrectTitle()
        {
            // Arrange
            var homePage = new HomePage(_page, _baseUrl);
            
            // Act
            await homePage.NavigateAsync();
            
            // Assert
            var title = await _page.TitleAsync();
            Assert.IsTrue(title.Contains("Welcome to Wanderlust Journal"));
            
            var headingText = await homePage.GetHeadingTextAsync();
            Assert.AreEqual("Wanderlust Journal", headingText);
        }
        
        [TestMethod]
        public async Task CanNavigate_FromHome_ToJournalList()
        {
            // Arrange
            var homePage = new HomePage(_page, _baseUrl);
            
            // Act
            await homePage.NavigateAsync();
            await homePage.ClickViewAllEntriesAsync();
            
            // Assert
            var title = await _page.TitleAsync();
            Assert.IsTrue(title.Contains("Journal Entries"));
            
            var journalListPage = new JournalListPage(_page, _baseUrl);
            var pageHeading = await journalListPage.GetPageHeadingAsync();
            Assert.AreEqual("Journal Entries", pageHeading);
        }
        
        [TestMethod]
        public async Task CanNavigate_FromHome_ToCreateJournal()
        {
            // Arrange
            var homePage = new HomePage(_page, _baseUrl);
            
            // Act
            await homePage.NavigateAsync();
            await homePage.ClickAddNewMemoryAsync();
            
            // Assert
            var title = await _page.TitleAsync();
            Assert.IsTrue(title.Contains("Create Journal Entry"));
            
            var createPage = new JournalCreatePage(_page, _baseUrl);
            var pageHeading = await createPage.GetPageHeadingAsync();
            Assert.AreEqual("Create Journal Entry", pageHeading);
        }
        
        [TestMethod]
        public async Task CanNavigate_FromJournalList_ToCreateJournal()
        {
            // Arrange
            var journalListPage = new JournalListPage(_page, _baseUrl);
            
            // Act
            await journalListPage.NavigateAsync();
            await journalListPage.ClickCreateNewAsync();
            
            // Assert
            var title = await _page.TitleAsync();
            Assert.IsTrue(title.Contains("Create Journal Entry"));
            
            var createPage = new JournalCreatePage(_page, _baseUrl);
            var pageHeading = await createPage.GetPageHeadingAsync();
            Assert.AreEqual("Create Journal Entry", pageHeading);
        }
    }
}
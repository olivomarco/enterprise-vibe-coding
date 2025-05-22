using System.Threading.Tasks;
using Microsoft.Playwright;

namespace WanderlustJournal.E2ETests.Pages
{
    public class HomePage : BasePage
    {
        public HomePage(IPage page, string baseUrl = "https://localhost:5001") 
            : base(page, baseUrl)
        {
        }
        
        public override string PagePath => "/";
        
        private ILocator HeadingText => _page.Locator("h1.display-4");
        private ILocator ViewAllEntriesButton => _page.GetByRole(AriaRole.Link, new() { Name = "View All Entries" });
        private ILocator AddNewMemoryButton => _page.GetByRole(AriaRole.Link, new() { Name = "Add New Memory" });
        
        public async Task<string> GetHeadingTextAsync()
        {
            return await HeadingText.TextContentAsync() ?? string.Empty;
        }
        
        public async Task ClickViewAllEntriesAsync()
        {
            await ViewAllEntriesButton.ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
        
        public async Task ClickAddNewMemoryAsync()
        {
            await AddNewMemoryButton.ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace WanderlustJournal.E2ETests.Pages
{
    public class JournalListPage : BasePage
    {
        public JournalListPage(IPage page, string baseUrl = "https://localhost:5001") 
            : base(page, baseUrl)
        {
        }
        
        public override string PagePath => "/Journal/Index";
        
        private ILocator PageHeading => _page.Locator("h1");
        private ILocator CreateNewButton => _page.GetByRole(AriaRole.Link, new() { Name = "Create New" });
        private ILocator SearchInput => _page.Locator("input[name='SearchTerm']");
        private ILocator SearchButton => _page.GetByRole(AriaRole.Button, new() { Name = "Search" });
        private ILocator JournalEntries => _page.Locator(".card");
        private ILocator NoEntriesMessage => _page.GetByText("No journal entries found.");
        
        public async Task<string> GetPageHeadingAsync()
        {
            return await PageHeading.TextContentAsync() ?? string.Empty;
        }
        
        public async Task ClickCreateNewAsync()
        {
            await CreateNewButton.ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
        
        public async Task SearchForEntryAsync(string searchTerm)
        {
            await SearchInput.FillAsync(searchTerm);
            await SearchButton.ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
        
        public async Task<int> GetEntryCountAsync()
        {
            return await JournalEntries.CountAsync();
        }
        
        public async Task<bool> HasNoEntriesMessageAsync()
        {
            return await NoEntriesMessage.IsVisibleAsync();
        }
        
        public async Task ClickEntryDetailsAsync(int index)
        {
            await JournalEntries.Nth(index).GetByText("Details").ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
        
        public async Task ClickEntryEditAsync(int index)
        {
            await JournalEntries.Nth(index).GetByText("Edit").ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
        
        public async Task ClickEntryDeleteAsync(int index)
        {
            await JournalEntries.Nth(index).GetByText("Delete").ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
        
        public async Task<List<string>> GetEntryTitlesAsync()
        {
            var count = await GetEntryCountAsync();
            var titles = new List<string>();
            
            for (int i = 0; i < count; i++)
            {
                var titleElement = JournalEntries.Nth(i).Locator("h5");
                var title = await titleElement.TextContentAsync();
                if (!string.IsNullOrEmpty(title))
                {
                    titles.Add(title);
                }
            }
            
            return titles;
        }
    }
}
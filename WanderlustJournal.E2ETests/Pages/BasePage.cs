using System.Threading.Tasks;
using Microsoft.Playwright;

namespace WanderlustJournal.E2ETests.Pages
{
    public abstract class BasePage
    {
        protected readonly IPage _page;
        public string BaseUrl { get; private set; }
        
        public BasePage(IPage page, string baseUrl = "https://localhost:5001")
        {
            _page = page;
            BaseUrl = baseUrl;
        }
        
        public abstract string PagePath { get; }
        
        public virtual async Task NavigateAsync()
        {
            await _page.GotoAsync($"{BaseUrl}{PagePath}");
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
        
        public async Task<string> GetPageTitleAsync()
        {
            return await _page.TitleAsync();
        }
    }
}
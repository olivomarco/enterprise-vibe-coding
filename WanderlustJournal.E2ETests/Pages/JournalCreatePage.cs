using System;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace WanderlustJournal.E2ETests.Pages
{
    public class JournalCreatePage : BasePage
    {
        public JournalCreatePage(IPage page, string baseUrl = "https://localhost:5001") 
            : base(page, baseUrl)
        {
        }
        
        public override string PagePath => "/Journal/Create";
        
        private ILocator PageHeading => _page.Locator("h1");
        private ILocator TitleInput => _page.Locator("input[id='JournalEntry_Title']");
        private ILocator LocationInput => _page.Locator("input[id='JournalEntry_Location']");
        private ILocator DateVisitedInput => _page.Locator("input[id='JournalEntry_DateVisited']");
        private ILocator NotesTextArea => _page.Locator("textarea[id='JournalEntry_Notes']");
        private ILocator SubmitButton => _page.GetByRole(AriaRole.Button, new() { Name = "Create" });
        private ILocator ValidationErrors => _page.Locator(".validation-summary-errors");
        
        public async Task<string> GetPageHeadingAsync()
        {
            return await PageHeading.TextContentAsync() ?? string.Empty;
        }
        
        public async Task FillEntryFormAsync(string title, string location, DateTime dateVisited, string notes)
        {
            await TitleInput.FillAsync(title);
            await LocationInput.FillAsync(location);
            await DateVisitedInput.FillAsync(dateVisited.ToString("yyyy-MM-dd"));
            await NotesTextArea.FillAsync(notes);
        }
        
        public async Task SubmitFormAsync()
        {
            await SubmitButton.ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
        
        public async Task<bool> HasValidationErrorsAsync()
        {
            return await ValidationErrors.IsVisibleAsync();
        }
        
        public async Task<string> GetValidationErrorsTextAsync()
        {
            if (await HasValidationErrorsAsync())
            {
                return await ValidationErrors.TextContentAsync() ?? string.Empty;
            }
            return string.Empty;
        }
    }
}
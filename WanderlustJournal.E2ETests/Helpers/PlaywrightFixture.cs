using System;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace WanderlustJournal.E2ETests.Helpers
{
    public class PlaywrightFixture : IDisposable
    {
        private bool _disposed = false;
        
        public IPlaywright Playwright { get; private set; }
        public IBrowser Browser { get; private set; }
        public IBrowserContext Context { get; private set; }
        public IPage Page { get; private set; }
        
        public PlaywrightFixture()
        {
            InitializePlaywright().GetAwaiter().GetResult();
        }
        
        private async Task InitializePlaywright()
        {
            Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true,
            });
            
            Context = await Browser.NewContextAsync(new BrowserNewContextOptions
            {
                IgnoreHTTPSErrors = true,
                ViewportSize = new ViewportSize { Width = 1920, Height = 1080 }
            });
            
            Page = await Context.NewPageAsync();
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
                
            if (disposing)
            {
                Context?.CloseAsync().GetAwaiter().GetResult();
                Browser?.CloseAsync().GetAwaiter().GetResult();
                Playwright?.Dispose();
            }
            
            _disposed = true;
        }
    }
}
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using WanderlustJournal.Models;
using WanderlustJournal.Pages.Journal;
using WanderlustJournal.Services;
using Xunit;

namespace WanderlustJournal.Tests.Pages.Journal
{
    public class CreateModelTests
    {
        [Fact]
        public void OnGet_InitializesJournalEntry()
        {
            // Arrange
            var journalEntryServiceMock = new Mock<JournalEntryService>(
                Mock.Of<WanderlustJournal.Data.JournalContext>(), 
                Mock.Of<GeocodingService>());
            
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            
            var pageModel = new CreateModel(journalEntryServiceMock.Object, webHostEnvironmentMock.Object);
            
            // Act
            pageModel.OnGet();
            
            // Assert
            Assert.NotNull(pageModel.JournalEntry);
            Assert.Equal(DateTime.Today, pageModel.JournalEntry.DateVisited.Date);
        }
        
        [Fact]
        public async Task OnPostAsync_WithValidModel_RedirectsToIndex()
        {
            // Arrange
            var journalEntryServiceMock = new Mock<JournalEntryService>(
                Mock.Of<WanderlustJournal.Data.JournalContext>(), 
                Mock.Of<GeocodingService>());
            
            journalEntryServiceMock.Setup(svc => svc.AddEntryAsync(It.IsAny<JournalEntry>()))
                .ReturnsAsync((JournalEntry entry) => 
                {
                    entry.Id = 1;
                    return entry;
                });
            
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            webHostEnvironmentMock.Setup(env => env.WebRootPath)
                .Returns("/tmp/wwwroot");
            
            var httpContext = new DefaultHttpContext();
            var modelState = new ModelStateDictionary();
            var actionContext = new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(), new PageActionDescriptor(), modelState);
            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
            var tempData = new TempDataDictionary(httpContext, Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());
            var pageContext = new PageContext(actionContext)
            {
                ViewData = viewData
            };
            
            var pageModel = new CreateModel(journalEntryServiceMock.Object, webHostEnvironmentMock.Object)
            {
                PageContext = pageContext,
                TempData = tempData,
                JournalEntry = new JournalEntry
                {
                    Title = "Test Title",
                    Location = "Test Location",
                    DateVisited = DateTime.Today,
                    Notes = "Test Notes"
                }
            };
            
            // Act
            var result = await pageModel.OnPostAsync();
            
            // Assert
            var redirectToPageResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("./Index", redirectToPageResult.PageName);
            journalEntryServiceMock.Verify(svc => svc.AddEntryAsync(It.IsAny<JournalEntry>()), Times.Once);
        }
        
        [Fact]
        public async Task OnPostAsync_WithInvalidModel_ReturnsPageResult()
        {
            // Arrange
            var journalEntryServiceMock = new Mock<JournalEntryService>(
                Mock.Of<WanderlustJournal.Data.JournalContext>(), 
                Mock.Of<GeocodingService>());
            
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            
            var pageModel = new CreateModel(journalEntryServiceMock.Object, webHostEnvironmentMock.Object);
            pageModel.ModelState.AddModelError("JournalEntry.Title", "Required");
            
            // Act
            var result = await pageModel.OnPostAsync();
            
            // Assert
            Assert.IsType<PageResult>(result);
            journalEntryServiceMock.Verify(svc => svc.AddEntryAsync(It.IsAny<JournalEntry>()), Times.Never);
        }
    }
}
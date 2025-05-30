using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using WanderlustJournal.Pages;
using Xunit;

namespace WanderlustJournal.Tests.Pages
{
    public class IndexModelTests
    {
        [Fact]
        public void OnGet_SetsViewDataTitle()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<IndexModel>>();
            var pageModel = new IndexModel(loggerMock.Object);
            
            // Setup ViewData dictionary manually since the PageModel's ViewData is read-only
            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
            var pageContext = new PageContext { ViewData = viewData };
            pageModel.PageContext = pageContext;
            
            // Act
            pageModel.OnGet();

            // Assert
            Assert.Equal("Welcome to Wanderlust Journal", pageModel.ViewData["Title"]);
        }
    }
}
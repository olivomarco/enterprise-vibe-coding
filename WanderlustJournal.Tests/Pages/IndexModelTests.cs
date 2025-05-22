using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
            var pageContext = new PageContext();
            pageModel.PageContext = pageContext;
            
            // Note: ViewData is automatically initialized by PageModel base class

            // Act
            pageModel.OnGet();

            // Assert
            Assert.Equal("Welcome to Wanderlust Journal", pageModel.ViewData["Title"]);
        }
    }
}
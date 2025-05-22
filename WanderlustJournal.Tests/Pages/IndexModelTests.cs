using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
            var pageModel = new IndexModel();
            var pageContext = new PageContext();
            pageModel.PageContext = pageContext;
            pageModel.ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary(
                new Microsoft.AspNetCore.Mvc.ModelBinding.EmptyModelMetadataProvider(),
                new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary())
            {
                Model = pageModel
            };

            // Act
            pageModel.OnGet();

            // Assert
            Assert.Equal("Welcome to Wanderlust Journal", pageModel.ViewData["Title"]);
        }
    }
}
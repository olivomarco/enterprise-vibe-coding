using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using WanderlustJournal.Models;
using WanderlustJournal.Pages.Journal;
using WanderlustJournal.Services;
using Xunit;

namespace WanderlustJournal.Tests.Pages.Journal
{
    public class IndexModelTests
    {
        [Fact]
        public void OnGet_NoSearchString_ReturnsAllEntries()
        {
            // Arrange
            var entries = new List<JournalEntry>
            {
                new JournalEntry { Id = 1, Title = "Paris", Location = "France" },
                new JournalEntry { Id = 2, Title = "Tokyo", Location = "Japan" }
            };
            
            // Create a mock service directly
            var journalEntryServiceMock = new Mock<IJournalEntryService>();
            
            journalEntryServiceMock.Setup(svc => svc.GetAllEntries())
                .Returns(entries);
                
            var pageModel = new IndexModel(journalEntryServiceMock.Object);
            
            // Act
            pageModel.OnGet();
            
            // Assert
            Assert.Equal(2, pageModel.JournalEntries.Count);
            Assert.Equal("Paris", pageModel.JournalEntries[0].Title);
            Assert.Equal("Tokyo", pageModel.JournalEntries[1].Title);
            Assert.Equal(string.Empty, pageModel.SearchString);
        }
        
        [Fact]
        public void OnGet_WithSearchString_ReturnsFilteredEntries()
        {
            // Arrange
            var filteredEntries = new List<JournalEntry>
            {
                new JournalEntry { Id = 1, Title = "Paris", Location = "France" }
            };
            
            // Create a mock service directly
            var journalEntryServiceMock = new Mock<IJournalEntryService>();
            
            journalEntryServiceMock.Setup(svc => svc.GetAllEntries())
                .Returns(filteredEntries);
                
            var pageModel = new IndexModel(journalEntryServiceMock.Object);
            pageModel.SearchString = "Paris";
            
            // Act
            pageModel.OnGet();
            
            // Assert
            Assert.Single(pageModel.JournalEntries);
            Assert.Equal("Paris", pageModel.JournalEntries[0].Title);
            Assert.Equal("Paris", pageModel.SearchString);
        }
    }
}
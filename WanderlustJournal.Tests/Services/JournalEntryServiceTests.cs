using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using WanderlustJournal.Data;
using WanderlustJournal.Models;
using WanderlustJournal.Services;
using Xunit;

namespace WanderlustJournal.Tests.Services
{
    public class JournalEntryServiceTests
    {
        private readonly DbContextOptions<JournalContext> _contextOptions;

        public JournalEntryServiceTests()
        {
            _contextOptions = new DbContextOptionsBuilder<JournalContext>()
                .UseInMemoryDatabase(databaseName: "TestJournalDatabase_" + Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public void GetAllEntries_ReturnsAllEntriesOrderedByDateDescending()
        {
            // Arrange
            using var context = new JournalContext(_contextOptions);
            SeedDatabase(context);
            
            var geocodingServiceMock = new Mock<GeocodingService>(
                Mock.Of<IHttpClientFactory>(),
                Mock.Of<ILogger<GeocodingService>>());
            
            var service = new JournalEntryService(context, geocodingServiceMock.Object);

            // Act
            var result = service.GetAllEntries();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal("Paris Trip", result[0].Title); // Most recent date first
            Assert.Equal("Tokyo Adventure", result[1].Title);
            Assert.Equal("Grand Canyon Hike", result[2].Title);
        }

        [Fact]
        public void GetEntryById_ExistingId_ReturnsCorrectEntry()
        {
            // Arrange
            using var context = new JournalContext(_contextOptions);
            SeedDatabase(context);
            
            var geocodingServiceMock = new Mock<GeocodingService>(
                Mock.Of<IHttpClientFactory>(),
                Mock.Of<ILogger<GeocodingService>>());
            
            var service = new JournalEntryService(context, geocodingServiceMock.Object);

            // Act
            var result = service.GetEntryById(2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Id);
            Assert.Equal("Tokyo Adventure", result.Title);
        }

        [Fact]
        public void GetEntryById_NonExistingId_ReturnsNull()
        {
            // Arrange
            using var context = new JournalContext(_contextOptions);
            SeedDatabase(context);
            
            var geocodingServiceMock = new Mock<GeocodingService>(
                Mock.Of<IHttpClientFactory>(),
                Mock.Of<ILogger<GeocodingService>>());
            
            var service = new JournalEntryService(context, geocodingServiceMock.Object);

            // Act
            var result = service.GetEntryById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddEntryAsync_ValidEntry_AddsToDatabase()
        {
            // Arrange
            using var context = new JournalContext(_contextOptions);
            
            var geocodingServiceMock = new Mock<GeocodingService>(
                Mock.Of<IHttpClientFactory>(),
                Mock.Of<ILogger<GeocodingService>>());
            
            geocodingServiceMock.Setup(x => 
                x.GeocodeLocationAsync(It.IsAny<string>()))
                .ReturnsAsync((40.7128m, -74.0060m));
            
            var service = new JournalEntryService(context, geocodingServiceMock.Object);
            
            var newEntry = new JournalEntry
            {
                Title = "New York Visit",
                Location = "New York City, USA",
                DateVisited = DateTime.Now.AddDays(-10),
                Notes = "Fantastic skyline views from Empire State Building"
            };

            // Act
            var result = await service.AddEntryAsync(newEntry);

            // Assert
            Assert.NotEqual(0, result.Id); // ID should be assigned
            Assert.Equal(40.7128m, result.Latitude);
            Assert.Equal(-74.0060m, result.Longitude);
            
            var savedEntry = await context.JournalEntries.FindAsync(result.Id);
            Assert.NotNull(savedEntry);
            Assert.Equal("New York Visit", savedEntry.Title);
        }

        [Fact]
        public void DeleteEntry_ExistingId_RemovesFromDatabase()
        {
            // Arrange
            using var context = new JournalContext(_contextOptions);
            SeedDatabase(context);
            
            var geocodingServiceMock = new Mock<GeocodingService>(
                Mock.Of<IHttpClientFactory>(),
                Mock.Of<ILogger<GeocodingService>>());
            
            var service = new JournalEntryService(context, geocodingServiceMock.Object);
            var initialCount = context.JournalEntries.Count();

            // Act
            service.DeleteEntry(2);

            // Assert
            Assert.Equal(initialCount - 1, context.JournalEntries.Count());
            Assert.Null(context.JournalEntries.FirstOrDefault(e => e.Id == 2));
        }

        [Fact]
        public void SearchEntries_MatchingTitle_ReturnsMatchingEntries()
        {
            // Arrange
            using var context = new JournalContext(_contextOptions);
            SeedDatabase(context);
            
            var geocodingServiceMock = new Mock<GeocodingService>(
                Mock.Of<IHttpClientFactory>(),
                Mock.Of<ILogger<GeocodingService>>());
            
            var service = new JournalEntryService(context, geocodingServiceMock.Object);

            // Act
            var result = service.SearchEntries("Tokyo");

            // Assert
            Assert.Single(result);
            Assert.Equal("Tokyo Adventure", result[0].Title);
        }

        [Fact]
        public void SearchEntries_EmptySearchTerm_ReturnsAllEntries()
        {
            // Arrange
            using var context = new JournalContext(_contextOptions);
            SeedDatabase(context);
            
            var geocodingServiceMock = new Mock<GeocodingService>(
                Mock.Of<IHttpClientFactory>(),
                Mock.Of<ILogger<GeocodingService>>());
            
            var service = new JournalEntryService(context, geocodingServiceMock.Object);

            // Act
            var result = service.SearchEntries("");

            // Assert
            Assert.Equal(3, result.Count);
        }

        private void SeedDatabase(JournalContext context)
        {
            context.JournalEntries.AddRange(
                new JournalEntry
                {
                    Id = 1,
                    Title = "Paris Trip",
                    Location = "Paris, France",
                    DateVisited = DateTime.Now.AddDays(-5),
                    Notes = "Visited the Eiffel Tower",
                    Latitude = 48.8584m,
                    Longitude = 2.2945m
                },
                new JournalEntry
                {
                    Id = 2,
                    Title = "Tokyo Adventure",
                    Location = "Tokyo, Japan",
                    DateVisited = DateTime.Now.AddDays(-15),
                    Notes = "Explored Shibuya Crossing",
                    Latitude = 35.6762m,
                    Longitude = 139.6503m
                },
                new JournalEntry
                {
                    Id = 3,
                    Title = "Grand Canyon Hike",
                    Location = "Grand Canyon, USA",
                    DateVisited = DateTime.Now.AddDays(-30),
                    Notes = "Amazing views all around",
                    Latitude = 36.0544m,
                    Longitude = -112.2401m
                }
            );
            context.SaveChanges();
        }
    }
}
using System;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using WanderlustJournal.Data;
using Xunit;

namespace WanderlustJournal.Tests.Data
{
    public class DatabaseMigrationHelperTests
    {
        [Fact]
        public void EnsureDatabaseCreatedAndMigrated_CreatesDatabase()
        {
            // Arrange
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<JournalContext>()
                .UseSqlite(connection)
                .Options;

            var serviceProvider = new Mock<IServiceProvider>();
            var serviceScope = new Mock<IServiceScope>();
            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            var services = new Mock<IServiceProvider>();
            var logger = new Mock<ILogger>();

            serviceProvider.Setup(sp => sp.GetService(typeof(IServiceScopeFactory)))
                .Returns(serviceScopeFactory.Object);
            serviceScopeFactory.Setup(ssf => ssf.CreateScope())
                .Returns(serviceScope.Object);
            serviceScope.Setup(ss => ss.ServiceProvider)
                .Returns(services.Object);

            using (var context = new JournalContext(options))
            {
                services.Setup(s => s.GetService(typeof(JournalContext)))
                    .Returns(context);

                // Act
                DatabaseMigrationHelper.EnsureDatabaseCreatedAndMigrated(serviceProvider.Object, logger.Object);
            }

            // Assert - Using a fresh context to verify
            using (var context = new JournalContext(options))
            {
                Assert.True(context.Database.EnsureCreated());
                Assert.True(context.JournalEntries.Any()); // Check seed data
            }

            connection.Close();
        }

        [Fact]
        public void EnsureDatabaseCreatedAndMigrated_HandlesExceptions()
        {
            // Arrange
            var serviceProvider = new Mock<IServiceProvider>();
            var serviceScope = new Mock<IServiceScope>();
            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            var services = new Mock<IServiceProvider>();
            var logger = new Mock<ILogger>();

            serviceProvider.Setup(sp => sp.GetService(typeof(IServiceScopeFactory)))
                .Returns(serviceScopeFactory.Object);
            serviceScopeFactory.Setup(ssf => ssf.CreateScope())
                .Returns(serviceScope.Object);
            serviceScope.Setup(ss => ss.ServiceProvider)
                .Returns(services.Object);

            // Setup to throw exception
            services.Setup(s => s.GetService(typeof(JournalContext)))
                .Throws(new Exception("Test exception"));

            // Act & Assert
            var exception = Record.Exception(() => 
                DatabaseMigrationHelper.EnsureDatabaseCreatedAndMigrated(serviceProvider.Object, logger.Object));

            // Verify the method handled the exception and didn't throw
            Assert.Null(exception);

            // Verify the logger was called with error level
            logger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
                Times.Once);
        }
    }
}
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace WanderlustJournal.Data
{
    public static class DatabaseMigrationHelper
    {
        public static void EnsureDatabaseCreatedAndMigrated(IServiceProvider serviceProvider, ILogger logger)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<JournalContext>();
                    
                    // Ensure database is created
                    context.Database.EnsureCreated();
                    
                    // Apply custom migrations for SQLite
                    EnsureCoordinateColumnsExist(context, logger);
                    
                    // Make sure seed data is present
                    EnsureSeedData(context, logger);
                    
                    logger.LogInformation("Database setup completed successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while setting up the database.");
                }
            }
        }
        
        private static void EnsureCoordinateColumnsExist(JournalContext context, ILogger logger)
        {
            try
            {
                // Create a raw connection to run direct SQL commands safely
                var connection = context.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    connection.Open();
                }
                
                using (var command = connection.CreateCommand())
                {
                    // Check if Latitude column exists
                    command.CommandText = "SELECT COUNT(*) FROM pragma_table_info('JournalEntries') WHERE name='Latitude'";
                    var latitudeExists = Convert.ToInt32(command.ExecuteScalar()) > 0;
                    
                    if (!latitudeExists)
                    {
                        using (var alterCommand = connection.CreateCommand())
                        {
                            alterCommand.CommandText = "ALTER TABLE JournalEntries ADD COLUMN Latitude DECIMAL(10, 7) NULL";
                            alterCommand.ExecuteNonQuery();
                            logger.LogInformation("Added Latitude column to JournalEntries table");
                        }
                    }
                    
                    // Check if Longitude column exists
                    command.CommandText = "SELECT COUNT(*) FROM pragma_table_info('JournalEntries') WHERE name='Longitude'";
                    var longitudeExists = Convert.ToInt32(command.ExecuteScalar()) > 0;
                    
                    if (!longitudeExists)
                    {
                        using (var alterCommand = connection.CreateCommand())
                        {
                            alterCommand.CommandText = "ALTER TABLE JournalEntries ADD COLUMN Longitude DECIMAL(10, 7) NULL";
                            alterCommand.ExecuteNonQuery();
                            logger.LogInformation("Added Longitude column to JournalEntries table");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error ensuring coordinate columns exist");
            }
        }
        
        private static void EnsureSeedData(JournalContext context, ILogger logger)
        {
            // Check if there's any data in the database
            if (!context.JournalEntries.Any())
            {
                logger.LogInformation("Adding seed data to database");
                
                context.JournalEntries.AddRange(
                    new Models.JournalEntry
                    {
                        Location = "Paris, France",
                        Title = "Eiffel Tower Adventure",
                        DateVisited = new DateTime(2024, 2, 15),
                        Notes = "Amazing view from the top. The city lights at night were breathtaking!",
                        ImageFileName = null,
                        Latitude = 48.8584m,
                        Longitude = 2.2945m
                    },
                    new Models.JournalEntry
                    {
                        Location = "Kyoto, Japan",
                        Title = "Cherry Blossom Season",
                        DateVisited = new DateTime(2023, 4, 5),
                        Notes = "The gardens were filled with beautiful pink blossoms. A truly memorable experience.",
                        ImageFileName = null,
                        Latitude = 35.0116m,
                        Longitude = 135.7681m
                    },
                    new Models.JournalEntry
                    {
                        Location = "Grand Canyon, USA",
                        Title = "Hiking the South Rim",
                        DateVisited = new DateTime(2022, 7, 22),
                        Notes = "The scale of the canyon is impossible to capture in photos. Need to come back for a longer trip!",
                        ImageFileName = null,
                        Latitude = 36.0544m,
                        Longitude = -112.2401m
                    }
                );
                
                context.SaveChanges();
            }
        }
    }
}
using Microsoft.EntityFrameworkCore;
using WanderlustJournal.Models;
using System;
using System.Linq;
using WanderlustJournal.Data.Migrations;

namespace WanderlustJournal.Data
{
    public class JournalContext : DbContext
    {
        public JournalContext(DbContextOptions<JournalContext> options)
            : base(options)
        {
        }

        public DbSet<JournalEntry> JournalEntries { get; set; } = default!;
        
        public void ApplyCoordinatesMigration()
        {
            try
            {
                // Check if Latitude column exists before adding it
                var latitudeExists = Database.ExecuteSqlRaw(@"
                    SELECT COUNT(*) FROM pragma_table_info('JournalEntries') 
                    WHERE name='Latitude';") > 0;
                    
                if (!latitudeExists)
                {
                    Database.ExecuteSqlRaw("ALTER TABLE JournalEntries ADD COLUMN Latitude DECIMAL(10, 7) NULL;");
                }
                
                // Check if Longitude column exists before adding it
                var longitudeExists = Database.ExecuteSqlRaw(@"
                    SELECT COUNT(*) FROM pragma_table_info('JournalEntries') 
                    WHERE name='Longitude';") > 0;
                    
                if (!longitudeExists)
                {
                    Database.ExecuteSqlRaw("ALTER TABLE JournalEntries ADD COLUMN Longitude DECIMAL(10, 7) NULL;");
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"Error in ApplyCoordinatesMigration: {ex.Message}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure decimal precision for coordinates
            modelBuilder.Entity<JournalEntry>()
                .Property(e => e.Latitude)
                .HasColumnType("decimal(10, 7)");
                
            modelBuilder.Entity<JournalEntry>()
                .Property(e => e.Longitude)
                .HasColumnType("decimal(10, 7)");

            // Seed data
            modelBuilder.Entity<JournalEntry>().HasData(
                new JournalEntry
                {
                    Id = 1,
                    Location = "Paris, France",
                    Title = "Eiffel Tower Adventure",
                    DateVisited = new DateTime(2024, 2, 15),
                    Notes = "Amazing view from the top. The city lights at night were breathtaking!",
                    ImageFileName = null,
                    Latitude = 48.8584m,
                    Longitude = 2.2945m
                },
                new JournalEntry
                {
                    Id = 2,
                    Location = "Kyoto, Japan",
                    Title = "Cherry Blossom Season",
                    DateVisited = new DateTime(2023, 4, 5),
                    Notes = "The gardens were filled with beautiful pink blossoms. A truly memorable experience.",
                    ImageFileName = null,
                    Latitude = 35.0116m,
                    Longitude = 135.7681m
                },
                new JournalEntry
                {
                    Id = 3,
                    Location = "Grand Canyon, USA",
                    Title = "Hiking the South Rim",
                    DateVisited = new DateTime(2022, 7, 22),
                    Notes = "The scale of the canyon is impossible to capture in photos. Need to come back for a longer trip!",
                    ImageFileName = null,
                    Latitude = 36.0544m,
                    Longitude = -112.2401m
                }
            );
        }
    }
}
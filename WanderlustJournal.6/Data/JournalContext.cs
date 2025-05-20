using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using WanderlustJournal.Models;
using System;
using System.Linq;
using WanderlustJournal.Data.Migrations;

namespace WanderlustJournal.Data
{
    public class JournalContext : IdentityDbContext<ApplicationUser>
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

            // We'll seed data through the DatabaseMigrationHelper instead
            // This ensures proper order of creation (user first, then journal entries)
        }
    }
}
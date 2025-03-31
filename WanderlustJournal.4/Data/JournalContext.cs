using Microsoft.EntityFrameworkCore;
using WanderlustJournal.Models;

namespace WanderlustJournal.Data
{
    public class JournalContext : DbContext
    {
        public JournalContext(DbContextOptions<JournalContext> options)
            : base(options)
        {
        }

        public DbSet<JournalEntry> JournalEntries { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed data
            modelBuilder.Entity<JournalEntry>().HasData(
                new JournalEntry
                {
                    Id = 1,
                    Location = "Paris, France",
                    Title = "Eiffel Tower Adventure",
                    DateVisited = new DateTime(2024, 2, 15),
                    Notes = "Amazing view from the top. The city lights at night were breathtaking!",
                    ImageFileName = null
                },
                new JournalEntry
                {
                    Id = 2,
                    Location = "Kyoto, Japan",
                    Title = "Cherry Blossom Season",
                    DateVisited = new DateTime(2023, 4, 5),
                    Notes = "The gardens were filled with beautiful pink blossoms. A truly memorable experience.",
                    ImageFileName = null
                },
                new JournalEntry
                {
                    Id = 3,
                    Location = "Grand Canyon, USA",
                    Title = "Hiking the South Rim",
                    DateVisited = new DateTime(2022, 7, 22),
                    Notes = "The scale of the canyon is impossible to capture in photos. Need to come back for a longer trip!",
                    ImageFileName = null
                }
            );
        }
    }
}
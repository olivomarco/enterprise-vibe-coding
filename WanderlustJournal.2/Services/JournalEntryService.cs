using System;
using System.Collections.Generic;
using System.Linq;
using WanderlustJournal.Models;

namespace WanderlustJournal.Services
{
    public class JournalEntryService
    {
        private readonly List<JournalEntry> _entries = new List<JournalEntry>();
        private int _nextId = 1;

        public JournalEntryService()
        {
            // Add some sample data
            AddEntry(new JournalEntry 
            { 
                Location = "Paris, France", 
                Title = "Eiffel Tower Adventure", 
                DateVisited = new DateTime(2024, 2, 15), 
                Notes = "Amazing view from the top. The city lights at night were breathtaking!" 
            });
            
            AddEntry(new JournalEntry 
            { 
                Location = "Kyoto, Japan", 
                Title = "Cherry Blossom Season", 
                DateVisited = new DateTime(2023, 4, 5), 
                Notes = "The gardens were filled with beautiful pink blossoms. A truly memorable experience." 
            });
            
            AddEntry(new JournalEntry 
            { 
                Location = "Grand Canyon, USA", 
                Title = "Hiking the South Rim", 
                DateVisited = new DateTime(2022, 7, 22), 
                Notes = "The scale of the canyon is impossible to capture in photos. Need to come back for a longer trip!" 
            });
        }

        public List<JournalEntry> GetAllEntries() => _entries;

        public JournalEntry GetEntryById(int id) => _entries.FirstOrDefault(e => e.Id == id);

        public JournalEntry AddEntry(JournalEntry entry)
        {
            entry.Id = _nextId++;
            _entries.Add(entry);
            return entry;
        }

        public void UpdateEntry(JournalEntry entry)
        {
            var existingEntry = GetEntryById(entry.Id);
            if (existingEntry == null) return;

            existingEntry.Location = entry.Location;
            existingEntry.Title = entry.Title;
            existingEntry.DateVisited = entry.DateVisited;
            existingEntry.Notes = entry.Notes;
        }

        public void DeleteEntry(int id)
        {
            var entry = GetEntryById(id);
            if (entry != null)
            {
                _entries.Remove(entry);
            }
        }
        
        public List<JournalEntry> SearchEntries(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAllEntries();
                
            searchTerm = searchTerm.ToLower();
            
            return _entries
                .Where(e => e.Title.ToLower().Contains(searchTerm) || 
                           e.Location.ToLower().Contains(searchTerm) ||
                           e.Notes.ToLower().Contains(searchTerm))
                .OrderByDescending(e => e.DateVisited)
                .ToList();
        }
    }
}

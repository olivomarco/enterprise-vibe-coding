using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WanderlustJournal.Data;
using WanderlustJournal.Models;

namespace WanderlustJournal.Services
{
    public class JournalEntryService
    {
        private readonly JournalContext _context;

        public JournalEntryService(JournalContext context)
        {
            _context = context;
        }

        public List<JournalEntry> GetAllEntries() => _context.JournalEntries.OrderByDescending(e => e.DateVisited).ToList();

        public JournalEntry GetEntryById(int id) => _context.JournalEntries.FirstOrDefault(e => e.Id == id);

        public JournalEntry AddEntry(JournalEntry entry)
        {
            _context.JournalEntries.Add(entry);
            _context.SaveChanges();
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
            
            // Only update the image filename if a new one is provided
            if (!string.IsNullOrEmpty(entry.ImageFileName))
            {
                existingEntry.ImageFileName = entry.ImageFileName;
            }
            
            _context.SaveChanges();
        }

        public void DeleteEntry(int id)
        {
            var entry = GetEntryById(id);
            if (entry != null)
            {
                _context.JournalEntries.Remove(entry);
                _context.SaveChanges();
            }
        }
        
        public List<JournalEntry> SearchEntries(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAllEntries();
                
            searchTerm = searchTerm.ToLower();
            
            return _context.JournalEntries
                .Where(e => e.Title.ToLower().Contains(searchTerm) || 
                           e.Location.ToLower().Contains(searchTerm) ||
                           e.Notes.ToLower().Contains(searchTerm))
                .OrderByDescending(e => e.DateVisited)
                .ToList();
        }
    }
}

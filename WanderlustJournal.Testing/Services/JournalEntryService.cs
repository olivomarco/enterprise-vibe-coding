using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WanderlustJournal.Data;
using WanderlustJournal.Models;

namespace WanderlustJournal.Services
{
    public class JournalEntryService
    {
        private readonly JournalContext _context;
        private readonly GeocodingService _geocodingService;

        public JournalEntryService(JournalContext context, GeocodingService geocodingService)
        {
            _context = context;
            _geocodingService = geocodingService;
        }

        public List<JournalEntry> GetAllEntries() => _context.JournalEntries.OrderByDescending(e => e.DateVisited).ToList();

        public JournalEntry GetEntryById(int id) => _context.JournalEntries.FirstOrDefault(e => e.Id == id);
        
        public async Task<JournalEntry> AddEntryAsync(JournalEntry entry)
        {
            // Get coordinates for the location
            if (!string.IsNullOrWhiteSpace(entry.Location))
            {
                var (latitude, longitude) = await _geocodingService.GeocodeLocationAsync(entry.Location);
                entry.Latitude = latitude;
                entry.Longitude = longitude;
            }
            
            _context.JournalEntries.Add(entry);
            await _context.SaveChangesAsync();
            return entry;
        }
        
        // Keep the synchronous version for backward compatibility
        public JournalEntry AddEntry(JournalEntry entry)
        {
            var task = AddEntryAsync(entry);
            task.Wait();
            return task.Result;
        }

        public async Task UpdateEntryAsync(JournalEntry entry)
        {
            var existingEntry = GetEntryById(entry.Id);
            if (existingEntry == null) return;

            // If location has changed, update coordinates
            if (existingEntry.Location != entry.Location)
            {
                var (latitude, longitude) = await _geocodingService.GeocodeLocationAsync(entry.Location);
                entry.Latitude = latitude;
                entry.Longitude = longitude;
            }
            else
            {
                // Keep existing coordinates if location hasn't changed
                entry.Latitude = existingEntry.Latitude;
                entry.Longitude = existingEntry.Longitude;
            }

            existingEntry.Location = entry.Location;
            existingEntry.Title = entry.Title;
            existingEntry.DateVisited = entry.DateVisited;
            existingEntry.Notes = entry.Notes;
            existingEntry.Latitude = entry.Latitude;
            existingEntry.Longitude = entry.Longitude;
            
            // Only update the image filename if a new one is provided
            if (!string.IsNullOrEmpty(entry.ImageFileName))
            {
                existingEntry.ImageFileName = entry.ImageFileName;
            }
            
            await _context.SaveChangesAsync();
        }
        
        // Keep the synchronous version for backward compatibility
        public void UpdateEntry(JournalEntry entry)
        {
            var task = UpdateEntryAsync(entry);
            task.Wait();
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

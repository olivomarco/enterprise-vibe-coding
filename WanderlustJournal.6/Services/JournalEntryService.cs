using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WanderlustJournal.Data;
using WanderlustJournal.Models;

namespace WanderlustJournal.Services
{
    public class JournalEntryService
    {
        private readonly JournalContext _context;
        private readonly GeocodingService _geocodingService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JournalEntryService(JournalContext context, GeocodingService geocodingService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _geocodingService = geocodingService;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public List<JournalEntry> GetAllEntries() 
        {
            var userId = GetCurrentUserId();
            return _context.JournalEntries
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.DateVisited)
                .ToList();
        }

        public JournalEntry GetEntryById(int id) 
        {
            var userId = GetCurrentUserId();
            return _context.JournalEntries
                .FirstOrDefault(e => e.Id == id && e.UserId == userId);
        }
        
        public async Task<JournalEntry> AddEntryAsync(JournalEntry entry)
        {
            // Get coordinates for the location
            if (!string.IsNullOrWhiteSpace(entry.Location))
            {
                var (latitude, longitude) = await _geocodingService.GeocodeLocationAsync(entry.Location);
                entry.Latitude = latitude;
                entry.Longitude = longitude;
            }
            
            // Set the user ID for the entry
            entry.UserId = GetCurrentUserId();
            
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
            var userId = GetCurrentUserId();
            var entry = _context.JournalEntries.FirstOrDefault(e => e.Id == id && e.UserId == userId);
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
            var userId = GetCurrentUserId();
            
            return _context.JournalEntries
                .Where(e => e.UserId == userId && 
                    (e.Title.ToLower().Contains(searchTerm) || 
                    e.Location.ToLower().Contains(searchTerm) ||
                    e.Notes.ToLower().Contains(searchTerm)))
                .OrderByDescending(e => e.DateVisited)
                .ToList();
        }
    }
}

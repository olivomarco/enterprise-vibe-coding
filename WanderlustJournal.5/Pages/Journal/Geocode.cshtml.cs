using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using WanderlustJournal.Models;
using WanderlustJournal.Services;
using WanderlustJournal.Data;
using Microsoft.EntityFrameworkCore;

namespace WanderlustJournal.Pages.Journal
{
    public class GeocodeModel : PageModel
    {
        private readonly JournalContext _context;
        private readonly GeocodingService _geocodingService;
        private readonly ILogger<GeocodeModel> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private static string _processId = string.Empty;
        private static int _processedCount = 0;
        private static int _successCount = 0;
        private static int _totalCount = 0;

        public bool IsProcessing { get; private set; }
        public bool HasBeenProcessed { get; private set; }
        public int ProcessedCount { get; private set; }
        public int SuccessCount { get; private set; }
        public int FailedCount { get; private set; }
        public int TotalCount { get; private set; }

        // Simple class to store entry IDs and locations
        private class EntryInfo
        {
            public int Id { get; set; }
            public string Location { get; set; }
        }

        public GeocodeModel(
            JournalContext context,
            GeocodingService geocodingService,
            ILogger<GeocodeModel> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _context = context;
            _geocodingService = geocodingService;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void OnGet()
        {
            // Check if there's an active process
            if (!string.IsNullOrEmpty(_processId))
            {
                IsProcessing = true;
                ProcessedCount = _processedCount;
                SuccessCount = _successCount;
                TotalCount = _totalCount;
                FailedCount = ProcessedCount - SuccessCount;

                // If processing is complete, reset and show summary
                if (ProcessedCount >= TotalCount)
                {
                    IsProcessing = false;
                    HasBeenProcessed = true;
                    _processId = string.Empty;
                }
            }
            // Initialize HasBeenProcessed to false when no process is active
            else
            {
                HasBeenProcessed = false;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Start a new geocoding process
            _processId = Guid.NewGuid().ToString();
            _processedCount = 0;
            _successCount = 0;
            
            // Get all entries without coordinates
            var entries = await _context.JournalEntries
                .Where(e => e.Latitude == null || e.Longitude == null)
                .ToListAsync();
                
            _totalCount = entries.Count;
            
            if (_totalCount == 0)
            {
                // No entries to process
                HasBeenProcessed = true;
                return RedirectToPage();
            }
            
            // Get just the IDs of entries that need geocoding using our explicit type
            var entryIds = entries.Select(e => new EntryInfo { Id = e.Id, Location = e.Location }).ToList();
            
            // Start the background geocoding process - passing only the necessary data
            _ = ProcessEntriesAsync(entryIds, _processId);
            
            // Redirect to get so the user can see the progress
            IsProcessing = true;
            return RedirectToPage();
        }
        
        private async Task ProcessEntriesAsync(IEnumerable<EntryInfo> entryData, string processId)
        {
            try
            {
                // Use a separate scope for the background processing
                using var scope = _serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<JournalContext>();
                var geocodingService = scope.ServiceProvider.GetRequiredService<GeocodingService>();
                
                foreach (var item in entryData)
                {
                    // Check if the process was cancelled by another request
                    if (processId != _processId)
                        return;
                        
                    if (!string.IsNullOrWhiteSpace(item.Location))
                    {
                        // Find the entry in this context
                        var entry = await dbContext.JournalEntries.FindAsync(item.Id);
                        if (entry == null) continue;
                        
                        // Geocode the location
                        var result = await geocodingService.GeocodeLocationAsync(item.Location);
                        decimal? latitude = result.latitude;
                        decimal? longitude = result.longitude;
                        
                        if (latitude.HasValue && longitude.HasValue)
                        {
                            entry.Latitude = latitude;
                            entry.Longitude = longitude;
                            dbContext.Update(entry);
                            await dbContext.SaveChangesAsync();
                            _successCount++;
                        }
                    }
                    
                    _processedCount++;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk geocoding");
            }
        }
    }
}
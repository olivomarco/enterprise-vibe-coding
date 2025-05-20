using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WanderlustJournal.Models;
using WanderlustJournal.Services;

namespace WanderlustJournal.Pages.Journal
{
    public class IndexModel : AuthorizedPageModel
    {
        private readonly JournalEntryService _journalService;

        public List<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();
        
        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; } = string.Empty;
        
        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }

        public IndexModel(JournalEntryService journalService)
        {
            _journalService = journalService;
        }

        public void OnGet()
        {
            var entries = _journalService.GetAllEntries();
            
            // Apply search filtering if SearchString is provided
            if (!string.IsNullOrEmpty(SearchString))
            {
                entries = entries.Where(e => 
                    e.Title.Contains(SearchString, StringComparison.OrdinalIgnoreCase) || 
                    e.Location.Contains(SearchString, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }
            
            // Apply date range filtering
            if (StartDate.HasValue)
            {
                entries = entries.Where(e => e.DateVisited >= StartDate.Value).ToList();
            }
            
            if (EndDate.HasValue)
            {
                entries = entries.Where(e => e.DateVisited <= EndDate.Value).ToList();
            }
            
            JournalEntries = entries.OrderByDescending(e => e.DateVisited).ToList();
        }
    }
}

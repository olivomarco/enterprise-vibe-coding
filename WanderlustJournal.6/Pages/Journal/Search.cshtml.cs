using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WanderlustJournal.Models;
using WanderlustJournal.Services;

namespace WanderlustJournal.Pages.Journal
{
    public class SearchModel : AuthorizedPageModel
    {
        private readonly JournalEntryService _journalService;

        public List<JournalEntry> SearchResults { get; set; } = new List<JournalEntry>();
        
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }
        
        public bool HasSearched { get; set; } = false;

        public SearchModel(JournalEntryService journalService)
        {
            _journalService = journalService;
        }

        public void OnGet()
        {
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                HasSearched = true;
                SearchResults = _journalService.SearchEntries(SearchTerm);
            }
        }
    }
}

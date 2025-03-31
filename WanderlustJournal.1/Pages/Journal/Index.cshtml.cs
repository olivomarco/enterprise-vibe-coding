using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WanderlustJournal.Models;
using WanderlustJournal.Services;

namespace WanderlustJournal.Pages.Journal
{
    public class IndexModel : PageModel
    {
        private readonly JournalEntryService _journalService;

        public List<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();

        public IndexModel(JournalEntryService journalService)
        {
            _journalService = journalService;
        }

        public void OnGet()
        {
            JournalEntries = _journalService.GetAllEntries()
                .OrderByDescending(e => e.DateVisited)
                .ToList();
        }
    }
}

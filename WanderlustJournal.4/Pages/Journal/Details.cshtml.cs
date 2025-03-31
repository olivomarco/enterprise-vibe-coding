using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WanderlustJournal.Models;
using WanderlustJournal.Services;

namespace WanderlustJournal.Pages.Journal
{
    public class DetailsModel : PageModel
    {
        private readonly JournalEntryService _journalService;

        public JournalEntry JournalEntry { get; set; }

        public DetailsModel(JournalEntryService journalService)
        {
            _journalService = journalService;
        }

        public IActionResult OnGet(int id)
        {
            JournalEntry = _journalService.GetEntryById(id);
            
            if (JournalEntry == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
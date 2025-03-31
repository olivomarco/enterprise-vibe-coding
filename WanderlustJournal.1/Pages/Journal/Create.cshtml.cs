using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WanderlustJournal.Models;
using WanderlustJournal.Services;

namespace WanderlustJournal.Pages.Journal
{
    public class CreateModel : PageModel
    {
        private readonly JournalEntryService _journalService;

        [BindProperty]
        public JournalEntry JournalEntry { get; set; }

        public CreateModel(JournalEntryService journalService)
        {
            _journalService = journalService;
        }

        public void OnGet()
        {
            // Initialize with today's date
            JournalEntry = new JournalEntry
            {
                DateVisited = DateTime.Today
            };
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _journalService.AddEntry(JournalEntry);
            return RedirectToPage("./Index");
        }
    }
}
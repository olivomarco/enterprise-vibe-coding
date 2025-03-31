using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WanderlustJournal.Models;
using WanderlustJournal.Services;

namespace WanderlustJournal.Pages.Journal
{
    public class EditModel : PageModel
    {
        private readonly JournalEntryService _journalService;

        [BindProperty]
        public JournalEntry JournalEntry { get; set; }

        public EditModel(JournalEntryService journalService)
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

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _journalService.UpdateEntry(JournalEntry);

            return RedirectToPage("./Details", new { id = JournalEntry.Id });
        }
    }
}
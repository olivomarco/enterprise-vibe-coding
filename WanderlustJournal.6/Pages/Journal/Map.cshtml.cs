using System.Collections.Generic;
using WanderlustJournal.Models;
using WanderlustJournal.Services;

namespace WanderlustJournal.Pages.Journal
{
    public class MapModel : AuthorizedPageModel
    {
        private readonly JournalEntryService _journalService;

        public MapModel(JournalEntryService journalService)
        {
            _journalService = journalService;
        }

        public List<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();

        public void OnGet()
        {
            JournalEntries = _journalService.GetAllEntries();
        }
    }
}
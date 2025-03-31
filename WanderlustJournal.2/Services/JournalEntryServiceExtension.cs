using System.Collections.Generic;
using System.Linq;
using WanderlustJournal.Models;

namespace WanderlustJournal.Services
{
    public static class JournalEntryServiceExtension
    {
        public static List<JournalEntry> SearchEntries(this JournalEntryService service, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return service.GetAllEntries();
                
            searchTerm = searchTerm.ToLower();
            
            return service.GetAllEntries()
                .Where(e => e.Title.ToLower().Contains(searchTerm) || 
                           e.Location.ToLower().Contains(searchTerm) ||
                           e.Notes.ToLower().Contains(searchTerm))
                .OrderByDescending(e => e.DateVisited)
                .ToList();
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using WanderlustJournal.Models;

namespace WanderlustJournal.Services
{
    public interface IJournalEntryService
    {
        List<JournalEntry> GetAllEntries();
        JournalEntry GetEntryById(int id);
        Task<JournalEntry> AddEntryAsync(JournalEntry entry);
        JournalEntry AddEntry(JournalEntry entry);
        Task UpdateEntryAsync(JournalEntry entry);
        void UpdateEntry(JournalEntry entry);
        void DeleteEntry(int id);
        List<JournalEntry> SearchEntries(string searchTerm);
    }
}
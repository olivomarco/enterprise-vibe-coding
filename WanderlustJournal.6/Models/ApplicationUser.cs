using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace WanderlustJournal.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();
    }
}
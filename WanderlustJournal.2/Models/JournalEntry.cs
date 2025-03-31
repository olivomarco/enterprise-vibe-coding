using System;
using System.ComponentModel.DataAnnotations;

namespace WanderlustJournal.Models
{
    public class JournalEntry
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Please enter a location")]
        public string Location { get; set; }
        
        [Required(ErrorMessage = "Please enter a title")]
        public string Title { get; set; }
        
        [Required(ErrorMessage = "Please enter the date visited")]
        [Display(Name = "Date Visited")]
        [DataType(DataType.Date)]
        public DateTime DateVisited { get; set; }
        
        [Display(Name = "Notes")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
    }
}

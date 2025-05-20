using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace WanderlustJournal.Models
{
    public class JournalEntry
    {
        public int Id { get; set; }

        public string? UserId { get; set; }
        
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
        
        [Required(ErrorMessage = "Please enter a location")]
        public string Location { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Please enter a title")]
        public string Title { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Please enter the date visited")]
        [Display(Name = "Date Visited")]
        [DataType(DataType.Date)]
        public DateTime DateVisited { get; set; }
        
        [Display(Name = "Notes")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; } = string.Empty;
        
        [Display(Name = "Photo")]
        public string? ImageFileName { get; set; }
        
        [Display(Name = "Upload Photo")]
        [DataType(DataType.Upload)]
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        
        // New properties for geo-coordinates
        [Display(Name = "Latitude")]
        [Column(TypeName = "decimal(10, 7)")]
        public decimal? Latitude { get; set; }
        
        [Display(Name = "Longitude")]
        [Column(TypeName = "decimal(10, 7)")]
        public decimal? Longitude { get; set; }
    }
}

using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Hosting;
using WanderlustJournal.Models;
using WanderlustJournal.Services;

namespace WanderlustJournal.Pages.Journal
{
    public class CreateModel : PageModel
    {
        private readonly JournalEntryService _journalService;
        private readonly IWebHostEnvironment _environment;
        private readonly int _maxFileSize = 2 * 1024 * 1024; // 2MB
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

        [BindProperty]
        public JournalEntry JournalEntry { get; set; } = new JournalEntry();

        public CreateModel(JournalEntryService journalService, IWebHostEnvironment environment)
        {
            _journalService = journalService;
            _environment = environment;
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
            // Handle image upload if provided
            if (JournalEntry.ImageFile != null)
            {
                // Validate file size
                if (JournalEntry.ImageFile.Length > _maxFileSize)
                {
                    ModelState.AddModelError("JournalEntry.ImageFile", "The file size cannot exceed 2MB.");
                }

                // Validate file type
                var fileExtension = Path.GetExtension(JournalEntry.ImageFile.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(fileExtension) || !Array.Exists(_allowedExtensions, ext => ext == fileExtension))
                {
                    ModelState.AddModelError("JournalEntry.ImageFile", "Only image files (jpg, jpeg, png, gif) are allowed.");
                }
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Process image if provided
            if (JournalEntry.ImageFile != null)
            {
                // Get the file extension
                string fileExtension = Path.GetExtension(JournalEntry.ImageFile.FileName).ToLowerInvariant();
                
                // If the filename doesn't have an extension or it's invalid, use .jpg as default
                if (string.IsNullOrEmpty(fileExtension) || !Array.Exists(_allowedExtensions, ext => ext == fileExtension))
                {
                    fileExtension = ".jpg";
                }
                
                // Generate a unique filename to avoid overwriting, ensuring proper extension
                string uniqueFileName = $"travel_photo_{DateTime.Now:yyyyMMdd}_{Guid.NewGuid().ToString().Substring(0, 8)}{fileExtension}";
                
                // Get the uploads folder path
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                
                // Save the file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    JournalEntry.ImageFile.CopyTo(fileStream);
                }
                
                // Store the filename in the database
                JournalEntry.ImageFileName = uniqueFileName;
            }

            _journalService.AddEntry(JournalEntry);
            return RedirectToPage("./Index");
        }
    }
}
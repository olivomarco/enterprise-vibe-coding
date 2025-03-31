using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Hosting;
using WanderlustJournal.Models;
using WanderlustJournal.Services;

namespace WanderlustJournal.Pages.Journal
{
    public class EditModel : PageModel
    {
        private readonly JournalEntryService _journalService;
        private readonly IWebHostEnvironment _environment;
        private readonly int _maxFileSize = 2 * 1024 * 1024; // 2MB
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

        [BindProperty]
        public JournalEntry JournalEntry { get; set; } = new JournalEntry();
        
        [BindProperty]
        public string ExistingImageFileName { get; set; }

        public EditModel(JournalEntryService journalService, IWebHostEnvironment environment)
        {
            _journalService = journalService;
            _environment = environment;
        }

        public IActionResult OnGet(int id)
        {
            JournalEntry = _journalService.GetEntryById(id);

            if (JournalEntry == null)
            {
                return NotFound();
            }
            
            ExistingImageFileName = JournalEntry.ImageFileName;
            return Page();
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
                    return Page();
                }

                // Validate file type
                var fileExtension = Path.GetExtension(JournalEntry.ImageFile.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(fileExtension) || !Array.Exists(_allowedExtensions, ext => ext == fileExtension))
                {
                    ModelState.AddModelError("JournalEntry.ImageFile", "Only image files (jpg, jpeg, png, gif) are allowed.");
                    return Page();
                }
                
                // Get the file extension and ensure it's valid
                if (string.IsNullOrEmpty(fileExtension) || !Array.Exists(_allowedExtensions, ext => ext == fileExtension))
                {
                    fileExtension = ".jpg";
                }
                
                // Generate a unique filename
                string uniqueFileName = $"travel_photo_{DateTime.Now:yyyyMMdd}_{Guid.NewGuid().ToString().Substring(0, 8)}{fileExtension}";
                
                // Get the uploads folder path
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                
                // Save the file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    JournalEntry.ImageFile.CopyTo(fileStream);
                }
                
                // Delete old image if it exists
                if (!string.IsNullOrEmpty(ExistingImageFileName))
                {
                    string oldFilePath = Path.Combine(_environment.WebRootPath, "uploads", ExistingImageFileName);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        try
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                        catch
                        {
                            // Ignore deletion errors
                        }
                    }
                }
                
                // Update the filename
                JournalEntry.ImageFileName = uniqueFileName;
            }
            else if (string.IsNullOrEmpty(JournalEntry.ImageFileName))
            {
                // No new image uploaded, preserve existing filename
                JournalEntry.ImageFileName = ExistingImageFileName;
            }
            
            // Update the entry
            try
            {
                _journalService.UpdateEntry(JournalEntry);
                return RedirectToPage("./Details", new { id = JournalEntry.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error saving changes: {ex.Message}");
                return Page();
            }
        }
    }
}
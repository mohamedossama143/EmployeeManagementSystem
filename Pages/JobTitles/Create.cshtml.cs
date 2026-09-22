using EmployeeManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Pages.JobTitles
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.JobTitle JobTitle { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (await TitleExistsAsync(JobTitle.Title))
                ModelState.AddModelError(nameof(JobTitle.Title), "A job title with this name already exists.");

            if (!ModelState.IsValid)
                return Page();

            _context.JobTitles.Add(JobTitle);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Job title \"{JobTitle.Title}\" was created.";
            return RedirectToPage("Index");
        }

        private async Task<bool> TitleExistsAsync(string title)
        {
            return await _context.JobTitles.AnyAsync(j => j.Title.ToLower() == title.Trim().ToLower());
        }
    }
}

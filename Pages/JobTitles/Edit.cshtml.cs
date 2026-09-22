using EmployeeManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Pages.JobTitles
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.JobTitle JobTitle { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var jobTitle = await _context.JobTitles.FindAsync(id);
            if (jobTitle == null)
                return NotFound();

            JobTitle = jobTitle;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (await TitleExistsAsync(JobTitle.Title, JobTitle.Id))
                ModelState.AddModelError(nameof(JobTitle.Title), "A job title with this name already exists.");

            if (!ModelState.IsValid)
                return Page();

            try
            {
                _context.Attach(JobTitle).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Job title \"{JobTitle.Title}\" was updated.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.JobTitles.AnyAsync(j => j.Id == JobTitle.Id))
                    return NotFound();
                throw;
            }

            return RedirectToPage("Index");
        }

        private async Task<bool> TitleExistsAsync(string title, int excludeId)
        {
            return await _context.JobTitles.AnyAsync(j => j.Title.ToLower() == title.Trim().ToLower() && j.Id != excludeId);
        }
    }
}

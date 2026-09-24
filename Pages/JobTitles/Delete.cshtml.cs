using EmployeeManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Pages.JobTitles
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.JobTitle JobTitle { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var jobTitle = await _context.JobTitles
                .Include(j => j.Employees)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (jobTitle == null)
                return NotFound();

            JobTitle = jobTitle;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var jobTitle = await _context.JobTitles
                .Include(j => j.Employees)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (jobTitle == null)
                return NotFound();

            if (jobTitle.Employees != null && jobTitle.Employees.Count > 0)
            {
                TempData["Error"] = $"Cannot delete \"{jobTitle.Title}\" — {jobTitle.Employees.Count} employee(s) are still assigned to it. Reassign them first.";
                return RedirectToPage("Delete", new { id });
            }

            _context.JobTitles.Remove(jobTitle);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Job title \"{jobTitle.Title}\" was deleted.";
            return RedirectToPage("Index");
        }
    }
}

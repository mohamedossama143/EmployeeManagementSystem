using EmployeeManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Pages.JobTitles
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

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
    }
}

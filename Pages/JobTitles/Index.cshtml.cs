using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Pages.JobTitles
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Models.JobTitle> JobTitles { get; set; } = new List<Models.JobTitle>();

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.JobTitles
                .Include(j => j.Employees)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(Search))
                query = query.Where(j => j.Title.Contains(Search));

            JobTitles = await query.OrderBy(j => j.Title).ToListAsync();
        }
    }
}

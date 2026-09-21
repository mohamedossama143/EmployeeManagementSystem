using EmployeeManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search, int? departmentId, int? jobTitleId)
        {
            var query = _context.Employees
                .Include(e => e.Department)
                .Include(e => e.JobTitle)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(e => e.FullName.Contains(search) || e.Email.Contains(search));

            if (departmentId.HasValue)
                query = query.Where(e => e.DepartmentId == departmentId);

            if (jobTitleId.HasValue)
                query = query.Where(e => e.JobTitleId == jobTitleId);

            ViewBag.Search = search;
            ViewBag.Departments = new SelectList(_context.Departments, "Id", "Name", departmentId);
            ViewBag.JobTitles = new SelectList(_context.JobTitles, "Id", "Title", jobTitleId);

            return View(await query.ToListAsync());
        }
    }
}
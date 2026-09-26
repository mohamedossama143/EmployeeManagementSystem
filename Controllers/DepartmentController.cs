using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Department
        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.Departments
                .Include(d => d.Employees)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(d => d.Name.Contains(search));

            ViewBag.Search = search;

            var departments = await query
                .AsNoTracking()
                .OrderBy(d => d.Name)
                .ToListAsync();

            return View(departments);
        }

        // GET: /Department/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var department = await _context.Departments
                .Include(d => d.Employees)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department == null) return NotFound();
            return View(department);
        }

        // GET: /Department/Create
        public IActionResult Create() => View();

        // POST: /Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Department department)
        {
            if (!ModelState.IsValid) return View(department);

            if (await _context.Departments.AnyAsync(d => d.Name == department.Name))
            {
                ModelState.AddModelError(nameof(department.Name), "Department name already exists.");
                return View(department);
            }

            _context.Add(department);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Department created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Department/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var department = await _context.Departments.FindAsync(id);
            if (department == null) return NotFound();
            return View(department);
        }

        // POST: /Department/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Department department)
        {
            if (id != department.Id) return NotFound();
            if (!ModelState.IsValid) return View(department);

            if (await _context.Departments.AnyAsync(d => d.Name == department.Name && d.Id != id))
            {
                ModelState.AddModelError(nameof(department.Name), "Department name already exists.");
                return View(department);
            }

            try
            {
                _context.Update(department);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Departments.AnyAsync(d => d.Id == id)) return NotFound();
                throw;
            }

            TempData["Success"] = "Department updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Department/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var department = await _context.Departments
                .Include(d => d.Employees)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department == null) return NotFound();
            return View(department);
        }

        // POST: /Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null) return NotFound();

            if (await _context.Employees.AnyAsync(e => e.DepartmentId == id))
            {
                TempData["Error"] = "Cannot delete a department that has employees.";
                return RedirectToAction(nameof(Index));
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Department deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
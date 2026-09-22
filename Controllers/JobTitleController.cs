using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Controllers
{
    public class JobTitleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JobTitleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: JobTitle
        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.JobTitles
                .Include(j => j.Employees)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(j => j.Title.Contains(search));

            ViewBag.Search = search;

            return View(await query.OrderBy(j => j.Title).ToListAsync());
        }

        // GET: JobTitle/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var jobTitle = await _context.JobTitles
                .Include(j => j.Employees)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (jobTitle == null)
                return NotFound();

            return View(jobTitle);
        }

        // GET: JobTitle/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: JobTitle/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title")] JobTitle jobTitle)
        {
            if (await TitleExistsAsync(jobTitle.Title))
                ModelState.AddModelError(nameof(jobTitle.Title), "A job title with this name already exists.");

            if (ModelState.IsValid)
            {
                _context.Add(jobTitle);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Job title \"{jobTitle.Title}\" was created.";
                return RedirectToAction(nameof(Index));
            }

            return View(jobTitle);
        }

        // GET: JobTitle/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var jobTitle = await _context.JobTitles.FindAsync(id);
            if (jobTitle == null)
                return NotFound();

            return View(jobTitle);
        }

        // POST: JobTitle/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title")] JobTitle jobTitle)
        {
            if (id != jobTitle.Id)
                return NotFound();

            if (await TitleExistsAsync(jobTitle.Title, excludeId: jobTitle.Id))
                ModelState.AddModelError(nameof(jobTitle.Title), "A job title with this name already exists.");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jobTitle);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Job title \"{jobTitle.Title}\" was updated.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await JobTitleExistsAsync(jobTitle.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(jobTitle);
        }

        // GET: JobTitle/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var jobTitle = await _context.JobTitles
                .Include(j => j.Employees)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (jobTitle == null)
                return NotFound();

            return View(jobTitle);
        }

        // POST: JobTitle/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jobTitle = await _context.JobTitles
                .Include(j => j.Employees)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (jobTitle == null)
                return NotFound();

            if (jobTitle.Employees != null && jobTitle.Employees.Count > 0)
            {
                TempData["Error"] = $"Cannot delete \"{jobTitle.Title}\" — {jobTitle.Employees.Count} employee(s) are still assigned to it. Reassign them first.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            _context.JobTitles.Remove(jobTitle);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Job title \"{jobTitle.Title}\" was deleted.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> JobTitleExistsAsync(int id)
        {
            return await _context.JobTitles.AnyAsync(j => j.Id == id);
        }

        private async Task<bool> TitleExistsAsync(string title, int? excludeId = null)
        {
            var query = _context.JobTitles.Where(j => j.Title.ToLower() == title.Trim().ToLower());
            if (excludeId.HasValue)
                query = query.Where(j => j.Id != excludeId.Value);
            return await query.AnyAsync();
        }
    }
}

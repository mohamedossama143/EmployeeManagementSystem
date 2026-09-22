using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;
        private readonly string[] _allowedExtension = { ".jpg", ".jpeg", ".png" };
        public EmployeeController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnviroment = webHostEnvironment;
        }
        public async Task<IActionResult> Create()
        {
            await LoadDropDown();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            ModelState.Remove(nameof(employee.Department));
            ModelState.Remove(nameof(employee.JobTitle));
            if (!ModelState.IsValid)
            {
                await LoadDropDown(employee.DepartmentId, employee.JobTitleId);
                return View(employee);
            }
            if (employee.FeatureImage != null)
            {
                var inputFileExtension = Path.GetExtension(employee.FeatureImage.FileName).ToLower();
                if (!_allowedExtension.Contains(inputFileExtension))
                {
                    ModelState.AddModelError("", "Invalid Image Format!");
                    await LoadDropDown(employee.DepartmentId, employee.JobTitleId);
                    return View(employee);
                }
                employee.ProfileImagePath = await UploadFiletoFolder(employee.FeatureImage);
            }
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            await LoadDropDown(employee.DepartmentId, employee.JobTitleId);
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.Id)
                return BadRequest();
            ModelState.Remove(nameof(employee.Department));
            ModelState.Remove(nameof(employee.JobTitle));
            if (!ModelState.IsValid)
            {
                await LoadDropDown(employee.DepartmentId, employee.JobTitleId);
                return View(employee);
            }
            var currentEmployee = await _context.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            if (currentEmployee == null)
                return NotFound();
            if (employee.FeatureImage != null)
            {
                var inputFileExtension = Path.GetExtension(employee.FeatureImage.FileName).ToLower();
                if (!_allowedExtension.Contains(inputFileExtension))
                {
                    ModelState.AddModelError("", "Invalid Image Format!");
                    await LoadDropDown(employee.DepartmentId, employee.JobTitleId);
                    return View(employee);
                }
                employee.ProfileImagePath = await UploadFiletoFolder(employee.FeatureImage);
            }
            else
            {
                employee.ProfileImagePath = currentEmployee.ProfileImagePath;
            }
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> UploadFiletoFolder(IFormFile file)
        {
            var inputFileExtension = Path.GetExtension(file.FileName);
            var fileName = Guid.NewGuid().ToString() + inputFileExtension;
            var wwwRootPath = _webHostEnviroment.WebRootPath;
            var ImageFolderPath = Path.Combine(wwwRootPath, "images");
            if (!Directory.Exists(ImageFolderPath))
            {
                Directory.CreateDirectory(ImageFolderPath);
            }
            var filePath = Path.Combine(ImageFolderPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return "/images/" + fileName;
        }

        private async Task LoadDropDown(int? selectedDepartmentId = null, int? selectedJobTitleId = null)
        {
            ViewBag.DepartmentId = new SelectList(
                await _context.Departments.ToListAsync(),
                "Id",
                "Name", selectedDepartmentId
            );

            ViewBag.JobTitleId = new SelectList(
                await _context.JobTitles.ToListAsync(),
                "Id",
                "Title", selectedJobTitleId
            );
        }
    }
}
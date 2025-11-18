using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University.Models;

namespace University.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CoursesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }


        // عرض كل الكورسات (GET)
        public async Task<IActionResult> GetCourses()
        {
            return View(await _context.Courses.ToListAsync());
        }

        // تفاصيل كورس (GET)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
            if (course == null) return NotFound();
            return View(course);
        }

        // إنشاء كورس (GET)
        public IActionResult Create()
        {
            return View();
        }

        // إنشاء كورس (POST) ورفع الملفات
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course, IFormFile? uploadedFile)
        {
            if (ModelState.IsValid)
            {
                string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadPath);

                if (uploadedFile != null)
                {
                    string fileExt = Path.GetExtension(uploadedFile.FileName).ToLower();
                    string[] allowedExtensions = { ".pdf", ".xls", ".xlsx", ".doc", ".docx" };

                    if (!allowedExtensions.Contains(fileExt))
                    {
                        ModelState.AddModelError("", "الملف يجب أن يكون PDF أو Excel أو Word فقط.");
                        return View(course);
                    }

                    string fileName = Guid.NewGuid().ToString() + fileExt;
                    string filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await uploadedFile.CopyToAsync(stream);
                    }

                    course.FilePath = "/uploads/" + fileName;
                }

                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(GetCourses));
            }

            return View(course);
        }

        // تعديل كورس (GET)  — 
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();
            return View(course);

        }


        // تعديل كورس (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Course course)
        {
            if (id != course.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Courses.Any(e => e.Id == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(GetCourses));
            }
            return View(course);
        }

        // حذف كورس (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
            if (course == null) return NotFound();

            return View(course);
        }

        // حذف كورس (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(GetCourses));
        }


    }
}

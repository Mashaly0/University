using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University.Models;

namespace University.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> GetCourses()
        {
            return View(await _context.Courses.ToListAsync());
        }

        #region Course and Room
        public async Task<IActionResult> Assign()
        {
            ViewBag.Students = await _context.Students.ToListAsync();
            ViewBag.Courses = await _context.Courses.ToListAsync();
            ViewBag.Rooms = await _context.Rooms.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(int studentId, List<int> courseIds, int roomId)
        {
            var student = await _context.Students
                .Include(s => s.Courses)
                .Include(s => s.Rooms)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null) return NotFound();

            var selectedCourses = await _context.Courses
                .Where(c => courseIds.Contains(c.Id))
                .ToListAsync();

            var room = await _context.Rooms.FindAsync(roomId);

            if (room == null) return NotFound();

            // Initialize if null
            student.Courses ??= new List<Course>();
            student.Rooms ??= new List<Room>();

            // إضافة الكورسات الجديدة
            foreach (var course in selectedCourses)
            {
                if (!student.Courses.Contains(course))
                    student.Courses.Add(course);
            }

            // إضافة الروم
            if (!student.Rooms.Contains(room))
                student.Rooms.Add(room);

            await _context.SaveChangesAsync();

            TempData["Success"] = "تم ربط الطالب بالكورسات والروم بنجاح!";
            return RedirectToAction(nameof(GetCourses));

        }
        #endregion
    }
}


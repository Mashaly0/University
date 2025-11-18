using Microsoft.AspNetCore.Mvc;
using University.Models;
using Microsoft.EntityFrameworkCore;

namespace University.Controllers
{
    public class AboutController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AboutController(ApplicationDbContext context)
        {
            _context = context;
        }


        // صفحة About
        public async Task<IActionResult> Index()
        {
            // جلب بعض الكورسات لعرضها كأمثلة
            var courses = await _context.Courses
                .OrderByDescending(c => c.Id)
                .Take(4) // عرض 4 كورسات فقط
                .ToListAsync();

            return View(courses);
        }
    }
}

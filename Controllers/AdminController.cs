using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using University.Models;

namespace YourProject.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        public async Task<IActionResult> Approve(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.IsApproved = true;
                await _userManager.UpdateAsync(user);
            }
            return RedirectToAction("Index");
        }
    }
}

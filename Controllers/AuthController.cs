using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using University.Models;
using System.Threading.Tasks;

namespace University.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // صفحة التسجيل
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string email, string password , string fullname)
        {
            var user = new ApplicationUser { UserName = email, Email = email,FullName=fullname, IsApproved = true };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                ViewBag.Message = "تم التسجيل بنجاح.";
                return View();
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View();
        }

        // صفحة تسجيل الدخول
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError("", "المستخدم غير موجود");
                return View();
            }
/*
            if (!user.IsApproved)
            {
                ModelState.AddModelError("", "في انتظار موافقة الأدمن.");
                return View();
            }
*/
            var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError("", "بيانات الدخول غير صحيحة");
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}

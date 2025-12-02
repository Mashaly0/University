using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using University.Models;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace University.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        // صفحة التسجيل
        [HttpGet]
        public IActionResult Register()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تحميل صفحة التسجيل");
                TempData["ErrorMessage"] = "حدث خطأ غير متوقع. الرجاء المحاولة لاحقاً.";
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string email, string password, string fullname)
        {
            try
            {
                // التحقق من صحة المدخلات
                if (string.IsNullOrWhiteSpace(email) ||
                    string.IsNullOrWhiteSpace(password) ||
                    string.IsNullOrWhiteSpace(fullname))
                {
                    ModelState.AddModelError("", "جميع الحقول مطلوبة");
                    return View();
                }

                // التحقق من صحة الإيميل
                if (!IsValidEmail(email))
                {
                    ModelState.AddModelError("", "صيغة البريد الإلكتروني غير صحيحة");
                    return View();
                }

                // التحقق من قوة كلمة المرور
                if (password.Length < 6)
                {
                    ModelState.AddModelError("", "كلمة المرور يجب أن تكون على الأقل 6 أحرف");
                    return View();
                }

                // التحقق مما إذا كان المستخدم موجود مسبقاً
                var existingUser = await _userManager.FindByEmailAsync(email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "هذا البريد الإلكتروني مسجل بالفعل");
                    return View();
                }

                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = fullname,
                    IsApproved = true
                };

                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("تم إنشاء حساب جديد للمستخدم: {Email}", email);
                    TempData["SuccessMessage"] = "تم التسجيل بنجاح. يمكنك الآن تسجيل الدخول.";
                    return RedirectToAction("Login");
                }

                foreach (var error in result.Errors)
                {
                    var errorMessage = GetFriendlyErrorMessage(error.Code);
                    ModelState.AddModelError("", errorMessage);
                    _logger.LogWarning("خطأ في إنشاء الحساب للمستخدم {Email}: {Error}", email, error.Description);
                }

                return View();
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "قيمة فارغة في بيانات التسجيل");
                ModelState.AddModelError("", "الرجاء تعبئة جميع الحقول المطلوبة");
                return View();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "عملية غير صحيحة في التسجيل");
                TempData["ErrorMessage"] = "حدث خطأ في عملية التسجيل. الرجاء المحاولة مرة أخرى.";
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ غير متوقع في عملية التسجيل للمستخدم: {Email}", email);
                TempData["ErrorMessage"] = "حدث خطأ غير متوقع. الرجاء المحاولة لاحقاً.";
                return View();
            }
        }

        // صفحة تسجيل الدخول
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            try
            {
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تحميل صفحة تسجيل الدخول");
                TempData["ErrorMessage"] = "حدث خطأ غير متوقع. الرجاء المحاولة لاحقاً.";
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, string returnUrl = null)
        {
            try
            {
                // التحقق من صحة المدخلات
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    ModelState.AddModelError("", "البريد الإلكتروني وكلمة المرور مطلوبان");
                    return View();
                }

                // البحث عن المستخدم
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    // لا نكشف أن المستخدم غير موجود لأسباب أمنية
                    _logger.LogWarning("محاولة دخول بحساب غير موجود: {Email}", email);
                    ModelState.AddModelError("", "البريد الإلكتروني أو كلمة المرور غير صحيحة");
                    return View();
                }

                // التحقق من حالة الموافقة
                if (!user.IsApproved)
                {
                    _logger.LogWarning("محاولة دخول بحساب غير موافق عليه: {Email}", email);
                    ModelState.AddModelError("", "الحساب قيد الانتظار للموافقة عليه");
                    return View();
                }

                // التحقق من حالة الحساب (محظور/مقفل)
                if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.Now)
                {
                    _logger.LogWarning("محاولة دخول بحساب مقفل: {Email}", email);
                    ModelState.AddModelError("", "الحساب مقفل مؤقتاً. الرجاء المحاولة لاحقاً");
                    return View();
                }

                // محاولة تسجيل الدخول
                var result = await _signInManager.PasswordSignInAsync(
                    user,
                    password,
                    false,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    _logger.LogInformation("تم تسجيل الدخول بنجاح للمستخدم: {Email}", email);

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                else if (result.IsLockedOut)
                {
                    _logger.LogWarning("الحساب مقفل بعد محاولات فاشلة: {Email}", email);
                    ModelState.AddModelError("", "تم قفل الحساب مؤقتاً بسبب عدة محاولات فاشلة. الرجاء المحاولة بعد 5 دقائق");
                    return View();
                }
                else if (result.IsNotAllowed)
                {
                    _logger.LogWarning("الدخول غير مسموح به للمستخدم: {Email}", email);
                    ModelState.AddModelError("", "الدخول غير مسموح به لهذا الحساب");
                    return View();
                }
                else if (result.RequiresTwoFactor)
                {
                    // إعادة التوجيه للتحقق بخطوتين إذا كان مفعلاً
                    return RedirectToAction("LoginWith2fa", new { returnUrl });
                }
                else
                {
                    // لا نعطي تفاصيل دقيقة عن سبب الفشل لأسباب أمنية
                    _logger.LogWarning("معلومات دخول غير صحيحة للمستخدم: {Email}", email);
                    ModelState.AddModelError("", "البريد الإلكتروني أو كلمة المرور غير صحيحة");
                    return View();
                }
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "قيمة فارغة في بيانات الدخول");
                ModelState.AddModelError("", "الرجاء إدخال البريد الإلكتروني وكلمة المرور");
                return View();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "عملية غير صحيحة في تسجيل الدخول");
                TempData["ErrorMessage"] = "حدث خطأ في عملية تسجيل الدخول. الرجاء المحاولة مرة أخرى.";
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ غير متوقع في عملية تسجيل الدخول للمستخدم: {Email}", email);
                ModelState.AddModelError("", "حدث خطأ غير متوقع. الرجاء المحاولة مرة أخرى.");
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userName = User.Identity?.Name;
                await _signInManager.SignOutAsync();
                _logger.LogInformation("تم تسجيل الخروج للمستخدم: {UserName}", userName);
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تسجيل الخروج");
                // حتى في حالة الخطأ، نحاول توجيه المستخدم لصفحة تسجيل الدخول
                return RedirectToAction("Login");
            }
        }

        // وظائف مساعدة
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private string GetFriendlyErrorMessage(string errorCode)
        {
            return errorCode switch
            {
                "DuplicateUserName" => "اسم المستخدم مسجل بالفعل",
                "DuplicateEmail" => "البريد الإلكتروني مسجل بالفعل",
                "InvalidEmail" => "صيغة البريد الإلكتروني غير صحيحة",
                "InvalidUserName" => "اسم المستخدم غير صحيح",
                "PasswordTooShort" => "كلمة المرور قصيرة جداً",
                "PasswordRequiresNonAlphanumeric" => "كلمة المرور يجب أن تحتوي على رمز خاص واحد على الأقل",
                "PasswordRequiresDigit" => "كلمة المرور يجب أن تحتوي على رقم واحد على الأقل",
                "PasswordRequiresLower" => "كلمة المرور يجب أن تحتوي على حرف صغير واحد على الأقل",
                "PasswordRequiresUpper" => "كلمة المرور يجب أن تحتوي على حرف كبير واحد على الأقل",
                _ => "حدث خطأ أثناء إنشاء الحساب"
            };
        }
    }
}
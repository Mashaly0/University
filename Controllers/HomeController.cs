using Azure;
using Microsoft.AspNetCore.Localization;



public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // ÌáÈ ÃÍÏË ÇáßæÑÓÇÊ (Ãæ ßá ÇáßæÑÓÇÊ) áÚÑÖåÇ Ýí ÇáÕÝÍÉ ÇáÑÆíÓíÉ
        var courses = await _context.Courses
            .OrderByDescending(c => c.Id)
            .ToListAsync();

        return View(courses);
    }
    /*
    public async Task<IActionResult> About()
    {
        return View();
    } */
    public IActionResult SetLanguage(string culture, string returnUrl)
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        );

        return LocalRedirect(returnUrl);
    }

}


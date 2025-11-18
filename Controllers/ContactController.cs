using Microsoft.AspNetCore.Mvc;

public class ContactController : Controller
{
    private readonly EmailService _emailService;

    public ContactController(EmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Index(ContactViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        // Send message to you
        _emailService.SendEmail(
            to: "your-admin-email@gmail.com",
            subject: "New Contact Message",
            body: $"Name: {model.Name}<br>Email: {model.Email}<br>Message:<br>{model.Message}"
        );

        // Auto-reply to sender
        _emailService.SendEmail(
            to: model.Email,
            subject: "Thank you for contacting us",
            body: "We received your message and will respond soon."
        );

        ViewBag.Success = "Your message has been sent.";
        return View();
    }
}

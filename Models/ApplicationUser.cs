using Microsoft.AspNetCore.Identity;
using University.Models;
namespace University.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public bool IsApproved { get; set; } = true; // موافقة الأدمن
    }
}

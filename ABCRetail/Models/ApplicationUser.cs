using Microsoft.AspNetCore.Identity;


namespace ABCRetail.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
    }
}

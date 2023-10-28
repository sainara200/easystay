using Microsoft.AspNetCore.Identity;

namespace SlnWeb.IdentityApp
{
    public class ApplicationUser:IdentityUser
    {
        public string? nombres { get; set; }
    }
}

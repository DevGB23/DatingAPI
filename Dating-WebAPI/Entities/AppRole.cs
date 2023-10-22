using Microsoft.AspNetCore.Identity;

namespace Dating_WebAPI.Entities
{
    public class AppRole : IdentityRole<int>
    {
        public ICollection<AppUserRole>? UserRoles { get; set; }
    }
}
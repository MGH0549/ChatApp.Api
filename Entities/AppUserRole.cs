using Microsoft.AspNetCore.Identity;

namespace ChatApp.Entities
{
    public class AppUserRole:IdentityUserRole<int>
    {
        public AppUser? User { get; set; }
        public AppRoles? Role { get; set; }
    }
}
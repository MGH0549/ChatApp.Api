using Microsoft.AspNetCore.Identity;

namespace ChatApp.Entities
{
    public class AppRoles:IdentityRole<int>
    {
      public ICollection<AppUserRole>? UserRoles { get; set; }
    }
}
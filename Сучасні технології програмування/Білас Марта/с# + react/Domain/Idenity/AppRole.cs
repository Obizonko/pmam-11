using Microsoft.AspNetCore.Identity;

namespace Domain.Idenity;

public class AppRole : IdentityRole<long>
{
    public virtual ICollection<AppUser> AppUsers { get; set; }

    public AppRole() : base("User") { }

    public AppRole(string roleName) : base(roleName)
    {
    }
}

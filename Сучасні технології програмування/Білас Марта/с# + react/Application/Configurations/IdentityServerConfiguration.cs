using Domain.Idenity;

namespace Application.Configurations
{
    public static class IdentityServerConfiguration
    {
        public static IEnumerable<AppRole> GetRoles()
        {
            return new List<AppRole>
            {
                new AppRole(IdentityRoles.SuperAdmin),
                new AppRole(IdentityRoles.Admin),
                new AppRole(IdentityRoles.User)
            };
        }
    }
}

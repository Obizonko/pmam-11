using Microsoft.EntityFrameworkCore;

namespace WebApplication3.EfCore
{
    public class EF_DataContext : DbContext
    {
        public EF_DataContext(DbContextOptions options) : base(options) { }
        public DbSet<Registration> registrations { get; set; }

    }

}

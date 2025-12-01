using DataContext.Configuration;
using Domain;
using Domain.Idenity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataContext;

public class RestaurantDataContext : IdentityDbContext<AppUser, AppRole, long>
{
	public DbSet<AppUser> Users { get; set; }
	public DbSet<AppRole> Roles { get; set; }
	public DbSet<Customer> Customers { get; set; }
	public DbSet<Meal> Meals { get; set; }
	public DbSet<Order> Orders { get; set; }
	public DbSet<OrderRow> OrderRows { get; set; }
	public DbSet<MealCategory> MealCategories { get; set; }
	public DbSet<OrderStatus> OrderStatuses { get; set; }
	public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }

	public RestaurantDataContext(DbContextOptions options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.Entity<Customer>(opt =>
		{
			opt.HasKey(p => p.Id);
			opt.Property(p => p.FirstName).HasColumnType("nvarchar(128)");
		});

		builder.Entity<Order>(opt =>
		{
            opt.HasKey("Id");
			opt.HasMany(p => p.OrderRows).WithOne(); 
			opt.HasOne(p => p.Customer); 
			opt.HasMany(p => p.OrderStatusHistories)
				.WithOne(osh => osh.Order)
				.HasForeignKey(osh => osh.OrderId)
				.OnDelete(DeleteBehavior.Cascade); 
			opt.Property(p => p.AdditionalInfo).HasColumnType("nvarchar(2048)");
		});

		builder.Entity<AppUser>(b =>
		{
			b.ToTable("AppUsers", "dbo");
			b.Property(u => u.UserName).HasMaxLength(128);
			b.Property(u => u.NormalizedUserName).HasMaxLength(128);
			b.Property(u => u.Email).HasMaxLength(128);
			b.Property(u => u.NormalizedEmail).HasMaxLength(128);
		});

		builder.Entity<AppRole>(b =>
		{
			b.ToTable("AppRoles", "dbo");
			b.Property(t => t.Name).HasMaxLength(128);
		});

		builder.Entity<Meal>(opt =>
		{
			opt.HasKey(p => p.Id);
			opt.HasOne(p => p.Category);
		});

		builder.Entity<MealCategory>(opt =>
		{
			opt.HasKey(p => p.Id);
			opt.Property(p => p.Name).IsRequired().HasMaxLength(255);
		});

		builder.ApplyConfiguration(new OrderStatusHistoryConfiguration());
		builder.ApplyConfiguration(new OrderStatusConfiguration());

		base.OnModelCreating(builder);
	}
}

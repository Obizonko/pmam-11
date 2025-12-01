using Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace DataContext.Configuration;

public class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
{
	public void Configure(EntityTypeBuilder<OrderStatusHistory> builder)
	{
		builder.HasKey(osh => osh.Id);

		builder.HasOne(osh => osh.Order)
			.WithMany(o => o.OrderStatusHistories)
			.HasForeignKey(osh => osh.OrderId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(osh => osh.OrderStatus)
			.WithMany(os => os.OrderStatusHistories)
			.HasForeignKey(osh => osh.OrderStatusId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.HasOne(osh => osh.User)
			.WithMany()
			.HasForeignKey(osh => osh.ChangedById)
			.OnDelete(DeleteBehavior.Restrict);

		builder.Property(osh => osh.TimeOfChange)
			   .IsRequired();
	}
}

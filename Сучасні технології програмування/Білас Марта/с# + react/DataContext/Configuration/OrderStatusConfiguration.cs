using Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Application.Configurations;

namespace DataContext.Configuration;

public class OrderStatusConfiguration : IEntityTypeConfiguration<OrderStatus>
{
	public void Configure(EntityTypeBuilder<OrderStatus> builder)
	{
		builder.HasKey(os => os.Id);

		builder.Property(os => os.Name)
			   .IsRequired()
			   .HasMaxLength(128);

		builder.HasData(
			new OrderStatus { Id = 1, Name = OrderStatuses.New },
			new OrderStatus { Id = 2, Name = OrderStatuses.Confirmed },
			new OrderStatus { Id = 3, Name = OrderStatuses.Preparing },
			new OrderStatus { Id = 4, Name = OrderStatuses.OutForDelivery },
			new OrderStatus { Id = 5, Name = OrderStatuses.Delivered },
			new OrderStatus { Id = 6, Name = OrderStatuses.CanceledByCustomer },
			new OrderStatus { Id = 7, Name = OrderStatuses.CanceledByRestaurant },
			new OrderStatus { Id = 8, Name = OrderStatuses.Returned }
		);
	}
}

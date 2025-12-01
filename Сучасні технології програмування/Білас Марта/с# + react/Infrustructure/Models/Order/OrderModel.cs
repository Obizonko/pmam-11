using Infrastructure.Models.User;

namespace Infrastructure.Models.Order;

public class OrderModel
{
	public long Id { get; set; }
	public IEnumerable<OrderRowModel>? OrderRows { get; set; }
	public UserModel? Customer { get; set; }
	public double Total { get; set; }
	public OrderStatusModel? OrderStatus { get; set; }
	public DateTime OrderDate { get; set; }
}

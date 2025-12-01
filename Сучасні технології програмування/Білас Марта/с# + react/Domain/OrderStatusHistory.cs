using Domain.Idenity;

namespace Domain
{
	public class OrderStatusHistory
	{
		public long Id { get; set; }
		public long OrderId { get; set; }
		public Order Order { get; set; }
		public long OrderStatusId { get; set; }
		public OrderStatus OrderStatus { get; set; }
		public long ChangedById { get; set; }
		public AppUser User { get;set; }
		public DateTimeOffset TimeOfChange { get; set; }
	}
}

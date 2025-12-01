using Infrastructure.Models.Order;

namespace Infrastructure.Interfaces;

public interface IOrderService
{
	public bool AddOrUpdateOrder(Guid anonId, int mealId, int amount);
	public void DeleteOrderRow(Guid anonId, int mealId);
	public OrderModel GetOrder(Guid? anonId);
	OrderListResponse GetOrdersList(int skip, int take, bool requireTotalCount);
	bool PlaceOrder(Guid anonId, string name, string surname, string adress, string email, string phone, int paymentType, string additionalInfo);
	public int UptadeAmount(Guid anonId, int mealId, bool increment);
	Task<OrderWithStatusHistoryModel> GetOrderByIdWithStatusHistory(long orderId);
}

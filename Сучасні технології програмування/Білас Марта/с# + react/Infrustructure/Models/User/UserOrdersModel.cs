using Infrastructure.Models.Order;

namespace Infrastructure.Models.User;

public class UserOrdersModel
{
    public ICollection<OrderModel> Orders { get; set; }
}

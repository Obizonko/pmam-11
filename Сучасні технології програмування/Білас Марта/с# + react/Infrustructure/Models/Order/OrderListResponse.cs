using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models.Order;

public class OrderListResponse
{
    public IList<OrderModel> Orders { get; set; }
    public int TotalCount { get; set; }
}

using Application.Configurations;
using DataContext;
using Domain;
using Domain.Idenity;
using Infrastructure.Interfaces;
using Infrastructure.Models.Order;
using Infrastructure.Models.User;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class OrderService : IOrderService
{
	private readonly RestaurantDataContext _db;

	public OrderService(RestaurantDataContext db)
	{
		_db = db;
	}

	public bool AddOrUpdateOrder(Guid anonId, int mealId, int amount)
	{
		var meal = _db.Meals.FirstOrDefault(e => e.Id == mealId);

		if (meal == null) return false;

		var newOrderRow = new OrderRow
		{
			Meal = meal,
			Amount = amount,
			Price = meal.Price
		};

		var newOrderStatusId = _db.OrderStatuses
			.Where(s => s.Name == OrderStatuses.New)
			.Select(s => s.Id)
			.FirstOrDefault();

		if (newOrderStatusId == 0) return false;

		var existingOrder = _db.Orders
			.Include(o => o.OrderRows)
				.ThenInclude(o => o.Meal)
			.OrderBy(e => e.Id)
			.LastOrDefault(o => o.AnonId == anonId && !o.IsPaid);

		if (existingOrder == null)
		{
			var newOrder = new Order
			{
				AnonId = anonId,
				PaymentType = 0,
				IsPaid = false,
				OrderRows = new List<OrderRow> { newOrderRow },
				OrderStatusHistories = new List<OrderStatusHistory>()
			};

			_db.Orders.Add(newOrder);
			_db.SaveChanges();

			newOrder.OrderStatusHistories.Add(new OrderStatusHistory
			{
				OrderId = newOrder.Id,
				OrderStatusId = newOrderStatusId,
				ChangedById = 1,
				TimeOfChange = DateTimeOffset.UtcNow
			});

			newOrder.RecalculateTotal();
		}
		else
		{
			var orderRow = existingOrder.OrderRows
				.FirstOrDefault(or => or.Meal.Id == mealId);

			if (orderRow != null)
			{
				orderRow.Amount += amount;
			}
			else
			{
				existingOrder.OrderRows.Add(newOrderRow);
			}

			existingOrder.RecalculateTotal();
		}

		_db.SaveChanges();
		return true;
	}

	public async Task<OrderWithStatusHistoryModel> GetOrderByIdWithStatusHistory(long orderId)
	{
		var order = await _db.Orders
			.Include(o => o.OrderRows)
				.ThenInclude(o => o.Meal)
			.Include(o => o.Customer)
			.Include(o => o.OrderStatusHistories.OrderByDescending(osh => osh.TimeOfChange)) 
				.ThenInclude(osh => osh.OrderStatus)
			.Include(o => o.OrderStatusHistories)
				.ThenInclude(osh => osh.User)
			.FirstOrDefaultAsync(o => o.Id == orderId);

		return new OrderWithStatusHistoryModel
		{
			Id = order.Id,
			OrderRows = order.OrderRows?.Select(or => new OrderRowModel
			{
				Id = or.Id,
				Price = or.Price,
				Amount = or.Amount,
				MealName = or.Meal.Name,
				Weight = or.Meal.Weight,
				ImageUrl = or.Meal.ImageUrl
			}).ToList(),
			Customer = new UserModel
			{
				Id = order.Customer.Id,
				LastName = order.Customer.LastName,
				FirstName = order.Customer.FirstName,
				PhoneNumber = order.Customer.PhoneNumber,
				Address = order.Customer.Address
			},
			Total = order.Total,
			OrderDate = order.OrderDate,
			StatusHistory = order.OrderStatusHistories.Select(orsh => new OrderStatusHistoryModel
			{
				Id = orsh.Id,
				StatusName = orsh.OrderStatus?.Name,
				ChangedBy = new UserModel
				{
					Id = orsh.User.Id,
					LastName = orsh.User.LastName,
					FirstName = orsh.User.FirstName,
					PhoneNumber = orsh.User.PhoneNumber,
					Address = orsh.User.Address
				},
				TimeOfChange = orsh.TimeOfChange,
			}).ToList()
		};
	}

	public OrderModel GetOrder(Guid? anonId)
	{
		var order = _db.Orders?
			.Include(o => o.OrderRows)
				.ThenInclude(o => o.Meal)
			.OrderBy(e => e.Id)
			.LastOrDefault(e => e.AnonId == anonId && e.IsPaid == false);

		if (order == null)
			return new OrderModel();

		var orderModel = new OrderModel
		{
			Id = order.Id,
			OrderRows = order.OrderRows?.Select(or => new OrderRowModel
			{
				Id = or.Id,
				Price = or.Price,
				Amount = or.Amount,
				MealName = or.Meal.Name,
				Weight = or.Meal.Weight,
				ImageUrl = or.Meal.ImageUrl
			}).ToList()
		};
		return orderModel;
	}

	public OrderListResponse GetOrdersList(int skip, int take, bool requireTotalCount)
	{
		var query = _db.Orders?
			.Include(o => o.Customer)
			.Where(e => e.IsPaid == true);

		int totalCount = requireTotalCount ? query.Count() : -1;

		var orders = query
			.OrderByDescending(e => e.OrderDate)
			.Skip(skip)
			.Take(take)
			.ToList();

		var orderModels = orders.Select(order => new OrderModel
		{
			Id = order.Id,
			Customer = new UserModel
			{
				Id = order.Customer.Id,
				LastName = order.Customer.LastName,
				FirstName = order.Customer.FirstName,
				PhoneNumber = order.Customer.PhoneNumber,
				Address = order.Customer.Address
			},
			Total = order.Total,
			OrderDate = order.OrderDate
		}).ToList();

		return new OrderListResponse
		{
			Orders = orderModels,
			TotalCount = totalCount
		};
	}
	public int UptadeAmount(Guid anonId, int mealId, bool increment)
	{
		var orderRow = _db.Orders
			 .Where(o => o.AnonId == anonId && !o.IsPaid)
			 .OrderByDescending(o => o.Id)
			 .SelectMany(o => o.OrderRows)
			 .Include(or => or.Meal)
			 .FirstOrDefault(or => or.Meal.Id == mealId);

		if (orderRow != null && increment)
			orderRow.Amount += 1;
		else if (orderRow.Amount > 1)
			orderRow.Amount -= 1;

		var amount = orderRow.Amount;

		var order = _db.Orders
						.Include(o => o.OrderRows)
						.FirstOrDefault(o => o.AnonId == anonId && !o.IsPaid);

		if (order != null)
		{
			order.RecalculateTotal();
		}

		_db.SaveChanges();

		return amount;
	}

	public void DeleteOrderRow(Guid anonId, int mealId)
	{

		var order = _db.Orders?
			.Include(o => o.OrderRows)
			.ThenInclude(o => o.Meal)
			.OrderBy(e => e.Id)
			.LastOrDefault(e => e.AnonId == anonId && e.IsPaid == false);
		if (order != null)
		{
			var orderRowToDelete = order.OrderRows.FirstOrDefault(or => or.Meal.Id == mealId);

			if (orderRowToDelete != null)
			{
				order.OrderRows.Remove(orderRowToDelete);
			}

		}
		if (order?.OrderRows?.Count != 0)
			order.RecalculateTotal();
		else
			_db.Orders.Remove(order);

		_db.SaveChanges();
	}

	public bool PlaceOrder(Guid anonId, string name, string surname, string adress, string email, string phone, int paymentType, string additionalInfo)
	{
		var customer = new AppUser
		{
			FirstName = name,
			LastName = surname,
			Address = adress,
			Email = email,
			PhoneNumber = phone,
			EmailConfirmed = false,
			PhoneNumberConfirmed = false,
		};

		var order = _db.Orders
			.Include(o => o.OrderStatusHistories)
				.ThenInclude(osh => osh.OrderStatus)
			.OrderBy(e => e.Id)
			.LastOrDefault(o => o.AnonId == anonId && o.IsPaid == false);

		if (order == null) return false;

		order.Customer = customer;
		order.IsPaid = true;
		if (additionalInfo != null)
			order.AdditionalInfo = additionalInfo;
		order.PaymentType = (Domain.Enums.PaymentType)paymentType;

		if (order.OrderStatusHistories == null)
			order.OrderStatusHistories = new List<OrderStatusHistory>();

		var newOrderStatusId = _db.OrderStatuses
			.Where(s => s.Name == OrderStatuses.Confirmed)
			.Select(s => s.Id)
			.FirstOrDefault();

		if (newOrderStatusId == 0) return false;

		order.OrderStatusHistories.Add(new OrderStatusHistory
		{
			OrderId = order.Id,
			OrderStatusId = newOrderStatusId,
			ChangedById = 1,
			TimeOfChange = DateTimeOffset.UtcNow
		});

		_db.SaveChanges();
		return true;
	}
}

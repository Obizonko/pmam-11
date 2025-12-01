using Application.Configurations;
using Infrastructure.Interfaces;
using Infrastructure.Models.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace restaurant.Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OrderController : ControllerBase
	{
		public readonly IAnonCustomerService _anonCustomerService;
		public readonly IOrderService _orderService;
		public readonly IUserService _userService;
		public OrderController(IAnonCustomerService anonCustomerService, IOrderService orderService, IUserService userService)
		{
			_anonCustomerService = anonCustomerService;
			_orderService = orderService;
			_userService = userService;
		}

		[HttpGet("GetOrder")]
		public IActionResult GetOrder()
		{
			var anonId = _anonCustomerService.GetAnonCustomer();
			if (anonId == null)
				return Ok(new OrderModel());
			var model = _orderService.GetOrder(anonId);
			return Ok(model);
		}

		[Authorize(Roles = IdentityRoles.Admin + "," + IdentityRoles.SuperAdmin)]
		[HttpGet("GetOrderById/{OrderId}")]
		public async Task<IActionResult> GetOrderByIdAsync([FromRoute] long OrderId)
		{
			try
			{
				var order = await _orderService.GetOrderByIdWithStatusHistory(OrderId);
				return Ok(order);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[Authorize(Roles = IdentityRoles.User)]
		[HttpGet("{email}", Name = "GetUserOrders")]
		public async Task<IActionResult> GetUserPaidOrders(string email)
		{
			try
			{
				var userOrders = await _userService.GetUserOrdersByEmail(email);
				return Ok(userOrders);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("addMealToOrder")]
		public ActionResult AddMealToOrder([FromBody] AddMealModel addModel)
		{
			var anonimusId = _anonCustomerService.CreateAnonCustomer();

			var wasOrderAdded = _orderService.AddOrUpdateOrder(anonimusId, addModel.MealId, addModel.Amount);
			if (wasOrderAdded)
				return Ok("added to cart");
			return BadRequest("Failed to add meal to order. Please try again.");
		}

		[HttpPost("placeOrder")]
		public IActionResult PlaceOrder([FromBody] PlaceOrderModel model)
		{
			try
			{
				var anonId = _anonCustomerService.GetAnonCustomer();
				bool isOrderPlaces = false;
				if (ModelState.IsValid)
				{
					isOrderPlaces = _orderService.PlaceOrder(anonId.Value, model.Name, model.Surname, model.Address, model.Email, model.Phone, model.PaymentType, model.AdditionalInfo);
				}
				if (isOrderPlaces)
					return Ok();
				else
					return BadRequest($"An error occurred while plaicng order, please try again.");
			}
			catch (Exception ex)
			{
				return BadRequest($"An error occurred while plaicng order: {ex.Message}");
			}
		}

		[HttpPut("DecrementAmount")]
		public IActionResult DecrementAmount([FromBody] int mealId)
		{
			try
			{
				var anonId = _anonCustomerService.GetAnonCustomer();
				var amount = _orderService.UptadeAmount(anonId.Value, mealId, false);
				return Ok(amount);
			}
			catch (Exception ex)
			{
				return BadRequest($"An error occurred while decrement the amount: {ex.Message}");
			}
		}

		[HttpPut("IncrementAmount")]
		public IActionResult IncrementAmount([FromBody] int mealId)
		{
			try
			{
				var anonId = _anonCustomerService.GetAnonCustomer();
				var amount = _orderService.UptadeAmount(anonId.Value, mealId, true);
				return Ok(amount);
			}
			catch (Exception ex)
			{
				return BadRequest($"An error occurred while increment the amount: {ex.Message}");
			}
		}

		[HttpDelete("deleteOrderRow")]
		public IActionResult DeleteMeal([FromBody] int mealId)
		{
			var anonId = _anonCustomerService.GetAnonCustomer();
			_orderService.DeleteOrderRow(anonId.Value, mealId);
			return Ok();
		}
	}
}

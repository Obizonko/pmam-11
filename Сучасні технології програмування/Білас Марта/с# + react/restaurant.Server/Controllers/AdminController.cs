using Application.Configurations;
using Infrastructure.Interfaces;
using Infrastructure.Models.Menu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace restaurant.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        public readonly IOrderService _orderService;
        public readonly IMenuService _menuService;

        public AdminController(IMenuService menuService, IOrderService orderService)
        {
            _menuService = menuService;
            _orderService = orderService;
        }

        [Authorize]
        [HttpGet("getOrders")]
        public IActionResult GetOrdersList([FromQuery] int skip, int take, bool requireTotalCount)
        {
            try
            {
                var orders = _orderService.GetOrdersList(skip, take, requireTotalCount);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = IdentityRoles.Admin)]
        [HttpPost("addMealToMenu")]
        public IActionResult AddMeal([FromBody] MealModel newMeal)
        {
            try
            {
                _menuService.AddMealToMenu(newMeal.Name, newMeal.CategoryId, newMeal.Price, newMeal.Weight, newMeal.ImageUrl, newMeal.Description);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("updateMeal")]
        public IActionResult UpdateMeal([FromBody] MealModel newMeal)
        {
            try
            {
                _menuService.UpdateMeal(newMeal.Id, newMeal.Name, newMeal.CategoryId, newMeal.Price, newMeal.Weight, newMeal.ImageUrl, newMeal.Description);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("deleteMealToMenu")]
        public IActionResult DeleteMeal([FromBody] int mealId)
        {
            try
            {
                _menuService.DeleteMealFromMenu(mealId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

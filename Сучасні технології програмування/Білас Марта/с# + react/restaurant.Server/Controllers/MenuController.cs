using Microsoft.AspNetCore.Mvc;
using Infrastructure.Interfaces;
using Infrastructure.Models.Menu;

namespace restaurant.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MenuController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenuController( IMenuService mealService)
    {
        
        _menuService = mealService;
    }

    [HttpGet("GetCategories")]
    public IActionResult GetCategories()
    {
        IList<CategoryItemModel> categories = _menuService.GetCategories();

        if (categories == null)
            return NotFound();

        return Ok(categories);
    }

    [HttpGet("GetMeals")]
    public IActionResult GetMeals(int categoryId)
    {
        return Ok(_menuService.GetMeals(categoryId));
    }

    [HttpGet("getMealById")]
    public IActionResult GetMealById(int id)
    {
        return Ok(_menuService.GetMealById(id));
    }
}

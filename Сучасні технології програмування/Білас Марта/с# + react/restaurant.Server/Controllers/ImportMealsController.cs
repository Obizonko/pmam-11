using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace restaurant.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportMealsController : ControllerBase
    {
        private readonly IMealImportService _mealImportService;

        public ImportMealsController(IMealImportService mealImportService)
        {
            _mealImportService = mealImportService;
        }

        [HttpPost("ImportMeals")]
        public IActionResult ImportMeals(IFormFile postedFile)
        {
            using (var reader = new StreamReader(postedFile.OpenReadStream()))
            {
                var text = reader.ReadToEnd();
                _mealImportService.ImportMeals(text);
            }

            return Ok("Imported");
        }
    }
}

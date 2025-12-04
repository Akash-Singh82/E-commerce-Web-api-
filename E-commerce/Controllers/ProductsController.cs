using Microsoft.AspNetCore.Mvc;
using E_commerce.Application.Interfaces;
namespace E_commerce.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : Controller
    {
        private readonly IMyProductService _ps;
        public ProductsController(IMyProductService ps)
        {
            _ps = ps;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var result = await _ps.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                          "An unexpected error occurred.");

            }

        }
    }
}

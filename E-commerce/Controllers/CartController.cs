using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cs;
        public CartController(ICartService cs)
        {
            _cs = cs;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCart()
        {
            var cart = await _cs.GetCartAsync();
            return CreatedAtAction(nameof(GetCart), new { id = cart.Id }, cart);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCart(Guid id) => Ok(await _cs.GetCartAsync(id));


        [HttpPost("{id:guid}/items")]
        public async Task<IActionResult> AddItem(Guid id, AddCartItemDto dto)
        {
            await _cs.AddItemAsync(id, dto.ProductId, dto.Quantity);
            return NoContent();
        }
    }
}

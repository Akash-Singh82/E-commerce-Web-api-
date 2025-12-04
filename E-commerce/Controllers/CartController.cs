using Microsoft.AspNetCore.Mvc;
using E_commerce.Models;
using E_commerce.Dtos;
using E_commerce.Application.Interfaces;



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
            var cart = await _cs.CreateCartAsync();
            return CreatedAtAction(nameof(GetCart), new { id = cart.Id }, cart);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCart(Guid id) {
            var cart = await _cs.GetCartAsync(id);
            if (cart == null)
                return NotFound();
            var result = new CartDto
            {
                Id = cart.Id,
                CreatedAt = cart.CreatedAt,
                Items = cart.Items.Select(i=> new CartItemDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };

            return Ok(result);
        }


        [HttpPost("{id:guid}/items")]
        public async Task<IActionResult> AddItem(Guid id, AddCartItemDto dto)
        {
            await _cs.AddItemAsync(id, dto.ProductId, dto.Quantity);
            return NoContent();
        }
    }
}

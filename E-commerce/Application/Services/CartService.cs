using E_commerce.Application.Interfaces;
using E_commerce.Data;
using E_commerce.Models;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.Application.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _db;

        public CartService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Cart> CreateCartAsync()
        {
            var cart = new Cart();
            _db.Carts.Add(cart);
            await _db.SaveChangesAsync();
            return cart;
        }

        public async Task<Cart?> GetCartAsync(Guid cartId)
        {
            return await _db.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.Id == cartId);
        }

        public async Task AddItemAsync(Guid cartId, int productId,  int quantity)
        {
            var cart = await GetCartAsync(cartId);
            if (cart == null)
                throw new Exception("Cart not found");

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if(existingItem == null)
            {
                cart.Items.Add(new CartItem
                {
                    CartId = cartId,
                    ProductId = productId,
                    Quantity = quantity
                });
            }
            else
            {
                existingItem.Quantity += quantity;
            }

            await _db.SaveChangesAsync();
        }
    }
}

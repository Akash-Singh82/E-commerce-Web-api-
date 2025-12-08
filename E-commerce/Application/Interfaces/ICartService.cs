using E_commerce.Models;

namespace E_commerce.Application.Interfaces
{
    public interface ICartService
    {
        Task<Cart> CreateCartAsync();
        Task<Cart?> GetCartAsync(Guid cartId);
        Task AddItemAsync(Guid cartId, int productId, int quantity);
        Task<bool> DeleteCartAsync(Guid id);

    }
}

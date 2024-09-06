using food_fantacy.Data;
using food_fantacy.Models;
using Microsoft.EntityFrameworkCore;

namespace food_fantacy.Services
{
    public class ProductServices
    {
        private readonly AppDbContext _context;

        public ProductServices(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductsDetails>> ItemsForSaleByUserIdAsync(string userId)
        {
            return await _context.ProductsDetails.Where(p => p.UserId == userId).ToListAsync();
        }

        public async Task<ProductsDetails?> ItemByIdAsync(int id)
        {
            return await _context.ProductsDetails.FindAsync(id);
        }

        public async Task AddItemAsync(ProductsDetails product)
        {
            _context.ProductsDetails.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateItemAsync(ProductsDetails product)
        {
            _context.ProductsDetails.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteItemAsync(int id)
        {
            var product = await _context.ProductsDetails.FindAsync(id);
            if (product != null)
            {
                _context.ProductsDetails.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}

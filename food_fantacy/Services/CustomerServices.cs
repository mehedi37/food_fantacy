using food_fantacy.Data;
using food_fantacy.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace food_fantacy.Services
{
    public class CustomerServices
    {
        private readonly AppDbContext _context;

        public CustomerServices(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CustomerViewModel>> CustomersBySellerIdAsync(string sellerId)
        {
            // Fetch the necessary data without performing the aggregation
            var cartData = await _context.Cart
                .Where(c => c.IsPurchased && c.CartItemsDetails.Any(cd => cd.ProductsDetails.UserId == sellerId))
                .Include(c => c.CartItemsDetails)
                .ThenInclude(cd => cd.ProductsDetails)
                .Include(c => c.AppUser)
                .ToListAsync();

            // Perform the aggregation in memory
            var customers = cartData
                .GroupBy(c => c.UserId)
                .Select(g => new CustomerViewModel
                {
                    CustomerName = g.First().AppUser.Name,
                    TotalSpent = g.Sum(c => c.CartItemsDetails.Sum(cd => cd.Price * cd.Quantity))
                })
                .ToList();

            return customers;
        }
    }
}

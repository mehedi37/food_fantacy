using food_fantacy.Areas.Identity.Data;
using food_fantacy.Data;
using food_fantacy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace food_fantacy.Services
{
    public class CartServices
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CartServices(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<CartItemsDetails>> CartDetailsAsync(AppUser user)
        {
            var cart = await _context.Cart
                .Include(c => c.CartItemsDetails)
                .ThenInclude(cd => cd.ProductsDetails)
                .FirstOrDefaultAsync(c => c.UserId == user.Id && !c.IsPurchased);

            if (cart == null || cart.CartItemsDetails == null)
            {
                return new List<CartItemsDetails>();
            }

            return cart.CartItemsDetails.ToList();
        }

        public async Task<Cart?> CartByUserIdAsync(string userId)
        {
            var cart = await _context.Cart
                .Include(c => c.CartItemsDetails)
                .ThenInclude(cd => cd.ProductsDetails)
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsPurchased);

            if (cart == null || cart.CartItemsDetails == null)
            {
                return null;
            }
            return cart;
        }

        public async Task AddToCartAsync(string userId, int productId, int quantity)
        {
            var cart = await CartByUserIdAsync(userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    CartId = new int(),
                    UserId = userId,
                    IsPurchased = false
                };
                _context.Cart.Add(cart);
                await _context.SaveChangesAsync();
            }

            var CartItemsDetails = await _context.CartItemsDetails
                .FirstOrDefaultAsync(cd => cd.CartId == cart.CartId && cd.ProductId == productId);

            if (CartItemsDetails == null)
            {
                var product = await _context.ProductsDetails.FindAsync(productId);
                if (product == null)
                {
                    throw new Exception("Product not found");
                }

                CartItemsDetails = new CartItemsDetails
                {
                    CartDetailsId = new int(),
                    CartId = cart.CartId,
                    ProductId = productId,
                    Quantity = quantity,
                    Price = product.ProductPrice
                };
                _context.CartItemsDetails.Add(CartItemsDetails);
            }
            else
            {
                CartItemsDetails.Quantity += quantity;
                _context.CartItemsDetails.Update(CartItemsDetails);
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateCartAsync(int cartDetailsId, int quantity)
        {
            var CartItemsDetails = await _context.CartItemsDetails.FindAsync(cartDetailsId);
            if (CartItemsDetails != null)
            {
                CartItemsDetails.Quantity = quantity;
                _context.CartItemsDetails.Update(CartItemsDetails);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveFromCartAsync(int cartDetailsId)
        {
            var CartItemsDetails = await _context.CartItemsDetails.FindAsync(cartDetailsId);
            if (CartItemsDetails != null)
            {
                _context.CartItemsDetails.Remove(CartItemsDetails);
                await _context.SaveChangesAsync();

                var cart = await _context.Cart
                    .Include(c => c.CartItemsDetails)
                    .FirstOrDefaultAsync(c => c.CartId == CartItemsDetails.CartId);

                if (cart != null && !cart.CartItemsDetails.Any())
                {
                    _context.Cart.Remove(cart);
                    await _context.SaveChangesAsync();
                }
            }
        }
        public async Task<int> CartDetailsCountAsync(string userId)
        {
            var cart = await CartByUserIdAsync(userId);
            return cart?.CartItemsDetails.Count ?? 0;
        }

        public async Task ClearCartAsync(string userId)
        {
            var cart = await CartByUserIdAsync(userId);
            if (cart != null)
            {
                _context.CartItemsDetails.RemoveRange(cart.CartItemsDetails);
                _context.Cart.Remove(cart);
                await _context.SaveChangesAsync();
            }
        }
        public async Task PurchaseCartAsync(string userId)
        {
            var cart = await CartByUserIdAsync(userId);
            if (cart != null)
            {
                cart.IsPurchased = true;
                _context.Cart.Update(cart);
                await _context.SaveChangesAsync();
            }
        }
    }
}

using food_fantacy.Data;
using food_fantacy.Services;
using food_fantacy.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace food_fantacy.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        private readonly CartServices _cartServices;
        private readonly ProductServices _productServices;

        public CartController(AppDbContext context, CartServices cartServices, ProductServices productServices)
        {
            _context = context;
            _cartServices = cartServices;
            _productServices = productServices;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _cartServices.CartByUserIdAsync(userId);

            if (cart == null || !cart.CartItemsDetails.Any())
            {
                ViewBag.Message = "Your cart is empty.";
                return View(new CartViewModel());
            }

            var cartViewModel = new CartViewModel
            {
                CartId = cart.CartId,
                CartItemsDetails = cart.CartItemsDetails.ToList(),
                TotalPrice = cart.CartItemsDetails.Sum(cd => cd.Price * cd.Quantity)
            };

            return View(cartViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var product = await _context.ProductsDetails.FindAsync(productId);

            if (product.UserId == userId)
            {
                return BadRequest("You cannot add your own product to the cart.");
            }

            if (product == null || product.Stock < quantity)
            {
                return BadRequest("Insufficient stock or product not found.");
            }

            await _cartServices.AddToCartAsync(userId, productId, quantity);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCart(int cartDetailsId, int quantity)
        {
            if (quantity < 1)
            {
                quantity = 1;
            }

            // Retrieve the product from the database
            var cartDetail = await _context.CartItemsDetails.FindAsync(cartDetailsId);
            var product = await _context.ProductsDetails.FindAsync(cartDetail.ProductId);

            // Check if the quantity is greater than the product's stock
            if (quantity > product.Stock)
            {
                // Set the quantity to the product's stock
                quantity = product.Stock;
            }

            await _cartServices.UpdateCartAsync(cartDetailsId, quantity);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartDetailsId)
        {
            await _cartServices.RemoveFromCartAsync(cartDetailsId);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _cartServices.ClearCartAsync(userId);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Purchase()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _cartServices.CartByUserIdAsync(userId);

            if (cart == null || !cart.CartItemsDetails.Any())
            {
                return BadRequest("Your cart is empty.");
            }

            foreach (var cartDetail in cart.CartItemsDetails)
            {
                var product = await _context.ProductsDetails.FindAsync(cartDetail.ProductId);
                if (product == null || product.Stock < cartDetail.Quantity)
                {
                    return BadRequest("Insufficient stock for product: " + product?.ProductName);
                }

                product.Stock -= cartDetail.Quantity;
                await _productServices.UpdateItemAsync(product);
            }

            await _cartServices.PurchaseCartAsync(userId);
            return RedirectToAction("Index");
        }

    }
}

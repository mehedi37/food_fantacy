using food_fantacy.Areas.Identity.Data;
using food_fantacy.Data;
using food_fantacy.Models;
using food_fantacy.Services;
using food_fantacy.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace food_fantacy.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ProductServices _productServices;
        private readonly CustomerServices _customerServices;

        public HomeController(ILogger<HomeController> logger, AppDbContext context, UserManager<AppUser> userManager, ProductServices productServices, CustomerServices customerServices)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _productServices = productServices;
            _customerServices = customerServices;
        }


        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var products = await _context.ProductsDetails
                .Where(p => p.UserId != userId)
                .ToListAsync();
            return View(products);
        }


        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var products = string.IsNullOrEmpty(query)
                ? await _context.ProductsDetails
                    .Where(p => p.UserId != userId)
                    .ToListAsync()
                : await _context.ProductsDetails
                    .Where(p => p.ProductName.Contains(query) && p.UserId != userId)
                    .ToListAsync();

            return PartialView("Partials/_searchResults_partial", products);
        }


        public async Task<IActionResult> FoodDetails(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var product = await _context.ProductsDetails.FindAsync(id);

            if (product.UserId == userId)
            {
                return BadRequest("You cannot view your own product from here.");
            }

            if (product == null)
            {
                return NotFound();
            }

            var otherProducts = await _context.ProductsDetails
                .Where(p => p.UserId != userId && p.ProductId != id)
                .Take(4)
                .ToListAsync();

            var viewModel = new ProductDetailsViewModel
            {
                Product = product,
                RelatedProducts = otherProducts
            };

            return View(viewModel);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

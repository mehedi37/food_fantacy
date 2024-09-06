using food_fantacy.Models;

namespace food_fantacy.ViewModel
{
    public class CartViewModel
    {
        public int CartId { get; set; }
        public decimal TotalPrice { get; set; }
        public List<CartItemsDetails> CartItemsDetails { get; set; } = new List<CartItemsDetails>();
    }
}

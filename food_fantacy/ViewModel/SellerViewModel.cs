using food_fantacy.Models;

namespace food_fantacy.ViewModel
{
    public class SellerViewModel
    {
        public List<ProductsDetails> ItemsForSale { get; set; } = new List<ProductsDetails>();
        public List<CustomerViewModel> Customers { get; set; } = new List<CustomerViewModel>();
    }
}

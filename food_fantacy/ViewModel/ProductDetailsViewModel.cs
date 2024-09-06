using food_fantacy.Models;

namespace food_fantacy.ViewModel
{
    public class ProductDetailsViewModel
    {
        public required ProductsDetails Product { get; set; }
        public required List<ProductsDetails> RelatedProducts { get; set; }
    }
}

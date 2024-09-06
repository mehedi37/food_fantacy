using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace food_fantacy.Models
{
    public class CartItemsDetails
    {
        [Key]
        public required int CartDetailsId { get; set; }
        [ForeignKey("ProductsDetails")]
        public required int ProductId { get; set; }
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public ProductsDetails? ProductsDetails { get; set; }

        [Precision(12, 2)]
        public required decimal Price { get; set; }

        [ForeignKey("Cart")]
        public required int CartId { get; set; }
        [DeleteBehavior(DeleteBehavior.Cascade)]
        public Cart? Cart { get; set; }

        public required int Quantity { get; set; }
    }
}

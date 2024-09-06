using food_fantacy.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace food_fantacy.Models
{
    public class ProductsDetails
    {
        [Key]
        public required int ProductId { get; set; }
        public required decimal ProductPrice { get; set; }
        public required string ProductName { get; set; }
        public required string ProductImage { get; set; }
        public required string ProductDescription { get; set; }
        public required int Stock { get; set; }

        [ForeignKey("AppUser")]
        public required string UserId { get; set; }
        [DeleteBehavior(DeleteBehavior.Cascade)]
        public AppUser? AppUser { get; set; }
    }
}

using food_fantacy.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace food_fantacy.Models
{
    public class Cart
    {
        [Key]
        public required int CartId { get; set; }
        [ForeignKey("AppUser")]
        public required string UserId { get; set; }
        [DeleteBehavior(DeleteBehavior.Cascade)]
        public AppUser? AppUser { get; set; }
        public bool IsPurchased { get; set; } = false;
        public ICollection<CartItemsDetails> CartItemsDetails { get; set; } = new List<CartItemsDetails>();
    }
}

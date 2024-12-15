using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Unique.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemID { get; set; }

        [ForeignKey("ShoppingCart")]
        public int CartID { get; set; }

        [ForeignKey("Product")]
        public int ProductID { get; set; }

        [Required]
        public int Quantity { get; set; }

        [NotMapped]
        public decimal Price { get; set; }

        // Relationships
        public virtual ShoppingCart? ShoppingCart { get; set; }
        public virtual Product? Product { get; set; }
    }


}

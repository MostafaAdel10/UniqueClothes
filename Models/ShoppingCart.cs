using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Unique.Models
{
    public class ShoppingCart
    {
        [Key]
        public int CartID { get; set; }

        
        [Required]
        public string UserID { get; set; }

        // Relationships
        [ForeignKey("UserID")]
        [ValidateNever]
        public virtual ApplicationUser ApplicationUser { get; set; }

        
    }
}

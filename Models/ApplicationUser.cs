using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Unique.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string? Name { get; set; }

        // Relationships
        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();

        [InverseProperty("ApplicationUser")] // متطابقة مع الخاصية في ShoppingCart
        public virtual ShoppingCart? ShoppingCart { get; set; }

        public virtual ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
    }
}

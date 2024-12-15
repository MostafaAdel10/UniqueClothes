using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Unique.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        [Required, MaxLength(300)]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public int Stock { get; set; }

        [ForeignKey("Category")]
        public int CategoryID { get; set; }


        [NotMapped]
        public IFormFile? Image { get; set; }

        [Display(Name = "Image Url1")]
        public string? ImageURL1 { get; set; }

        [Display(Name = "Image Url2")]
        public string? ImageURL2 { get; set; }

        [Display(Name = "Image Url3")]
        public string? ImageURL3 { get; set; }

        public string Size { get; set; }
        public string Color { get; set; }

        // Relationships
        public virtual Category? Category { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new HashSet<OrderDetail>();
        public virtual ICollection<Review> Reviews { get; set; } = new HashSet<Review>(); // علاقة المراجعات
    }


}

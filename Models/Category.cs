using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Unique.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        public string? Description { get; set; }

        [NotMapped]
        public IFormFile? Image { get; set; }


        [Display(Name = "Image Url")]
        public string? ImageURL { get; set; }
        // Relationships
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }


}

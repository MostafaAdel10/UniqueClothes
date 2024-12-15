using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Unique.Models
{
    public class Review
    {
        [Key]
        public int ReviewID { get; set; }

        [Required]
        public string UserID { get; set; }

        [Required]
        [ForeignKey("Product")]
        public int ProductID { get; set; }

        [Required]
        [Range(1, 5)] // تقييم من 1 إلى 5
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string Comment { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("UserID")]
        public virtual ApplicationUser? User { get; set; }
        public virtual Product? Product { get; set; }
    }

}

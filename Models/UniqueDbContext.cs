using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Unique.Models
{
    public class UniqueDbContext : IdentityDbContext<ApplicationUser>
    {
        public UniqueDbContext(DbContextOptions<UniqueDbContext> options)
        : base(options)
        {
        }

        // DbSet properties
        //public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // One-to-One: User and ShoppingCart
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.ShoppingCart)
                .WithOne(sc => sc.ApplicationUser)
                .HasForeignKey<ShoppingCart>(sc => sc.UserID)
                .OnDelete(DeleteBehavior.Restrict);
            
            // One-to-Many: User and Order
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserID)
                .OnDelete(DeleteBehavior.Restrict);
            // One-to-Many: Category and Product
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-Many: Order and OrderDetails
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderID)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-Many: Product and OrderDetails
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductID)
                .OnDelete(DeleteBehavior.Restrict);

            // علاقة المراجعة مع المستخدم
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews) // إضافة مجموعة Reviews في كلاس User
                .HasForeignKey(r => r.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            // علاقة المراجعة مع المنتج
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Reviews) // إضافة مجموعة Reviews في كلاس Product
                .HasForeignKey(r => r.ProductID)
                .OnDelete(DeleteBehavior.Restrict); // عند حذف المنتج، احذف مراجعاته.

            base.OnModelCreating(modelBuilder);
        }
    }
}

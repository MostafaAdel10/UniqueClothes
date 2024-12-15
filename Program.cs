using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Stripe;
using Unique.Models;
using Unique.Repository;
using Unique.Utility;

namespace Unique
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //AddDbContext
            builder.Services.AddDbContext<UniqueDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("cs"));
            });

            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

            //Service AddIdentity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;

            }).AddEntityFrameworkStores<UniqueDbContext>()
              .AddDefaultTokenProviders();

            //Security
            builder.Services.ConfigureApplicationCookie(option =>
            {
                option.AccessDeniedPath = $"/Account/AccessDenied";
                option.LoginPath = $"/Account/Login";
                option.LogoutPath = $"/Account/Logout";
            });

            //builder.Services.AddAuthentication().AddFacebook(option =>
            //{
            //    option.AppId = "193813826680436";
            //    option.AppSecret = "8fc42ae3f4f2a4986143461d4e2da919";
            //});
            //builder.Services.AddAuthentication().AddMicrosoftAccount(option =>
            //{
            //    option.ClientId = "ec4d380d-d631-465d-b473-1e26ee706331";
            //    option.ClientSecret = "qMW8Q~LlEEZST~SDxDgcEVx_45LJQF2cQ_rEKcSQ";
            //});

            builder.Services.AddDistributedMemoryCache();


            builder.Services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(100);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            //Custom Service "Register"
            builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
            builder.Services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();

            builder.Services.AddScoped<IEmailSender, EmailSender>();
            builder.Services.AddRazorPages();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();
            app.MapRazorPages();
            StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

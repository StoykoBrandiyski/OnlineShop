using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;

namespace OnlineShop.Web.Data
{
    public class OnlineShopDbContext : IdentityDbContext<ShopUser, IdentityRole, string>
    {
        public DbSet<City> Cities { get; set; }

        public DbSet<Address> Addresses { get; set; }

        public DbSet<ParentCategory> ParentCategories { get; set; }

        public DbSet<SubCategory> SubCategories { get; set; }

        public DbSet<CategoryProduct> CategoryProducts { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderProduct> OrderProducts { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductImage> ProductImages { get; set; }

        public DbSet<ShoppingCart> ShoppingCarts { get; set; }

        public DbSet<ShoppingCartProduct> ShoppingCartProducts { get; set; }

        public DbSet<Supplier> Suppliers { get; set; }

        public DbSet<ShopUserFavoriteProduct> ShopUserFavoriteProducts { get; set; }
        

        public OnlineShopDbContext(DbContextOptions<OnlineShopDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<OrderProduct>().HasKey(x => new { x.OrderId, x.ProductId });
            builder.Entity<ShoppingCartProduct>().HasKey(x => new { x.ShoppingCartId, x.ProductId });

            builder.Entity<CategoryProduct>().HasKey(x => new { x.SubCategoryId, x.ProductId });

            builder.Entity<ShopUserFavoriteProduct>().HasKey(x => new { x.ShopUserId, x.ProductId});

            builder.Entity<Product>()
                .HasOne(x => x.SubCategory)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.SubCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ShoppingCart>()
                    .HasOne(x => x.ShopUser)
                    .WithOne(x => x.ShoppingCart)
                    .HasForeignKey<ShopUser>(x => x.ShoppingCartId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Company>()
                .HasOne(x => x.ShopUser)
                .WithOne(x => x.Company)
                .HasForeignKey<ShopUser>(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(builder);
        }
    }
}

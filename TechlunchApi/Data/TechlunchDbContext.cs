using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TechlunchApi.Models;

namespace TechlunchApi.Data
{
    public class TechlunchDbContext : IdentityDbContext
    {
        private readonly DbContextOptions _options;
        public TechlunchDbContext(DbContextOptions<TechlunchDbContext> options)
        : base(options)
        {
            _options = options;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        //public DbSet<Admin> Admins { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<FoodItem> FoodItems { get; set; }
        public DbSet<FoodItemIngredients> FoodItemIngredients { get; set; }
        public DbSet<GeneralInventory> GeneralInventory { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }

    }
}

using Microsoft.EntityFrameworkCore;
using TechlunchApi.Models;

namespace TechlunchApi.Data
{
    public class TechlunchDbContext : DbContext
    {
        public TechlunchDbContext(DbContextOptions<TechlunchDbContext> options)
        : base(options)
        {
        }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Ingredient> Ingredient { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<FoodItem> FoodItem { get; set; }
        public DbSet<FoodItemIngredients> FoodItemIngredients { get; set; }
        public DbSet<GeneralInventory> GeneralInventory { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }

    }
}

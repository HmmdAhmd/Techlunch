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
    }
}

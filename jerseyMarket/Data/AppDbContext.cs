using jerseyMarket.Models;
using Microsoft.EntityFrameworkCore;

namespace jerseyMarket.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
    }
}

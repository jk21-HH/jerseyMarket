using Microsoft.EntityFrameworkCore;

namespace jerseyMarket.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {

    }
}

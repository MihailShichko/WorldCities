using Microsoft.EntityFrameworkCore;
using WorldCities.Server.Models;

namespace WorldCities.Server.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions) : base(dbContextOptions) { }

        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }

    }
}

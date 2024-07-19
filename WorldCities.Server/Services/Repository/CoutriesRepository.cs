using Microsoft.EntityFrameworkCore;
using WorldCities.Server.Data;
using WorldCities.Server.Models;

namespace WorldCities.Server.Services.Repository
{
    public class CoutriesRepository : IRepository<Country>
    {
        private readonly ApplicationDbContext _context;

        public CoutriesRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool AddInstance(Country instance)
        {
            _context.Countries.Add(instance);
            return Save();
        }

        public bool DeleteInstance(int id)
        {
            var oldCountry = _context.Countries.FirstOrDefault(c => c.Id == id);
            _context.Countries.Remove(oldCountry);
            return Save();
        }

        public async Task<IEnumerable<Country>> GetAll()
        {
            return await _context.Countries.Include(c => c.Cities).ToListAsync();
        }

        public async Task<Country> GetByIdAsync(int id)
        {
            return await _context.Countries.FirstOrDefaultAsync(c => c.Id == id);
        }

        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }

        public bool UpdateInstance(Country instance)
        {
            var oldCountry = _context.Countries.FirstOrDefault(c => c.Id == instance.Id);
            oldCountry.Name = instance.Name;
            oldCountry.ISO2 = instance.ISO2;
            oldCountry.ISO3 = instance.ISO3;
            oldCountry.Cities = instance.Cities;
            return Save();
        }
    }
}

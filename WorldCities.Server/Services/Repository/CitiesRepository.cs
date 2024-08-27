using Microsoft.EntityFrameworkCore;
using WorldCities.Server.Data;
using WorldCities.Server.Models;

namespace WorldCities.Server.Services.Repository
{
    public class CitiesRepository : IRepository<City>
    {
        private readonly ApplicationDbContext _context;

        public CitiesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool AddInstance(City instance)
        {
            _context.Cities.Add(instance);
            return Save();
        }

        public bool DeleteInstance(int id)
        {
            var oldCity = _context.Cities.FirstOrDefault(c => c.Id == id);
            _context.Cities.Remove(oldCity);
            return Save();

        }

        public IQueryable<City> GetAll()
        {
            return _context.Cities.Include(c => c.Country);
        }

        public async Task<City> GetByIdAsync(int id)
        {
            return await _context.Cities.FirstOrDefaultAsync(c => c.Id == id);
        }

        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }

        public bool UpdateInstance(City instance)
        {
            var oldCity = _context.Cities.FirstOrDefault(city => city.Id == instance.Id);
            oldCity.Name = instance.Name;
            oldCity.Country = instance.Country;
            oldCity.Lat = instance.Lat;
            oldCity.CountryId = instance.CountryId;
            oldCity.Lon = instance.Lon;
            return Save();
        }
    }
}

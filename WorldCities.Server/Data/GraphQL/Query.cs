using WorldCities.Server.Models;
using WorldCities.Server.Services.Repository;

namespace WorldCities.Server.Data.GraphQL
{
    public class Query
    {
        /// <summary>
        /// Gets all Cities.
        /// </summary>
        [Serial]
        [UsePaging]
        [UseFiltering]
        [UseSorting]
        public IQueryable<City> GetCities([Service] CitiesRepository repository) => 
             repository.GetAll();

        /// <summary>
        /// Gets all Countries.
        /// </summary>
        [Serial]
        [UsePaging]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Country> GetCountries(
        [Service] CoutriesRepository repository)
        => repository.GetAll();
    }
}


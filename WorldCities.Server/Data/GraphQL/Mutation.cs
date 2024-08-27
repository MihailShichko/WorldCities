using HotChocolate.Authorization;
using WorldCities.Server.Models;
using WorldCities.Server.Models.DTO;
using WorldCities.Server.Services.Repository;

namespace WorldCities.Server.Data.GraphQL
{
    public class Mutation
    {
        /// <summary>
        /// Add a new City
        /// </summary>
        [Serial]
        [Authorize(Roles = ["RegisteredUser"])]
        public async Task<City> AddCity([Service] CitiesRepository repository, CityDTO cityDTO)
        {
            var city = new City
            {
                Id = cityDTO.Id,
                Name = cityDTO.Name,
                Lat = cityDTO.Lat,
                Lon = cityDTO.Lon,
                CountryId = cityDTO.CountryId
            };

            if (!repository.AddInstance(city))
            {

            }
            return city;
        }

        /// <summary>
        /// Update an existing City
        /// </summary>
        [Serial]
        [Authorize(Roles = ["RegisteredUser"])]
        public async Task<City> UpdateCity([Service] CitiesRepository repository, CityDTO cityDTO)
        {
            var city = new City
            {
                Id = cityDTO.Id,
                Name = cityDTO.Name,
                Lat = cityDTO.Lat,
                Lon = cityDTO.Lon,
                CountryId = cityDTO.CountryId
            };

            if(!repository.UpdateInstance(city))
            {

            }
            return city;
        }

        [Serial]
        [Authorize(Roles = ["Admin"])]
        public async Task<City> DeleteCity([Service] CitiesRepository repository, CityDTO cityDTO)
        {
            var city = new City
            {
                Id = cityDTO.Id,
                Name = cityDTO.Name,
                Lat = cityDTO.Lat,
                Lon = cityDTO.Lon,
                CountryId = cityDTO.CountryId
            };

            repository.DeleteInstance(city.Id);

            return city;
        }

        [Serial]
        [Authorize(Roles = ["RegisteredUser"])]
        public async Task<Country> AddCountry([Service] CoutriesRepository repository, CountryDTO countryDTO)
        {
            var country = new Country
            {
                Id = countryDTO.Id,
                Name = countryDTO.Name,
                ISO2 = countryDTO.ISO2,
                ISO3 = countryDTO.ISO3
            };

            if (!repository.AddInstance(country))
            {

            }
            return country;
        }

        /// <summary>
        /// Update an existing City
        /// </summary>
        [Serial]
        [Authorize(Roles = ["RegisteredUser"])]
        public async Task<Country> UpdateCountry([Service] CoutriesRepository repository, CountryDTO countryDTO)
        {
            var country = new Country
            {
                Id = countryDTO.Id,
                Name = countryDTO.Name,
                ISO2 = countryDTO.ISO2,
                ISO3 = countryDTO.ISO3
            };

            if (!repository.UpdateInstance(country))
            {

            }
            return country;
        }

        [Serial]
        [Authorize(Roles = ["Admin"])]
        public async Task<Country> DeleteCountry([Service] CoutriesRepository repository, CountryDTO countryDTO)
        {
            var country = new Country
            {
                Id = countryDTO.Id,
                Name = countryDTO.Name,
                ISO2 = countryDTO.ISO2,
                ISO3 = countryDTO.ISO3
            };

            repository.DeleteInstance(country.Id);

            return country;
        }

    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using WorldCities.Server.Data;
using WorldCities.Server.Models;
using WorldCities.Server.Models.DTO;
using WorldCities.Server.Services.Repository;

namespace WorldCities.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly CitiesRepository _repository;

        public CityController(IRepository<City> repository)
        {
            _repository = (CitiesRepository)repository;
        }

        // GET: api/City
        [HttpGet]
        public async Task<ActionResult<ApiResult<CityDTO>>> GetCities(int pageIndex = 0,
        int pageSize = 10, string? sortColumn = null, string? sortOrder = null, string? filterColumn = null, string? filterQuery = null)
        {
            var cities = await _repository.GetAll();
            return await ApiResult<CityDTO>.CreateAsync(cities.Select(c => new CityDTO()
            {
                Id = c.Id,
                Name = c.Name,
                CountryName = c.Country.Name == null ? "undefined" : c.Country.Name,
                Lat = c.Lat,
                Lon = c.Lon,
                CountryId = c.Id
            }), pageIndex, pageSize, sortColumn, sortOrder, filterColumn, filterQuery); 
        }

        // GET: api/City/5
        [HttpGet("{id}")]
        public async Task<ActionResult<City>> GetCity(int id)
        {
            var city = await _repository.GetByIdAsync(id);

            if (city == null)
            {
                return NotFound();
            }

            return city;
        }

        // PUT: api/City/5
        
        
        [HttpPut("{id}")]
        [Authorize(Roles = "RegistratedUser")]
        public async Task<ActionResult> PutCity(int id, City city)
        {
            if (id != city.Id)
            {
                return BadRequest();
            }

            if (!_repository.UpdateInstance(city))
            {
                return NotFound();
            }

            return NoContent();
        }


        [HttpPost]
        [Route("IsDupeCity")]
        public async Task<bool> IsDupeCity(City city)
        {
            var cities = await _repository.GetAll();
            return cities.Any(c => c.Name == city.Name 
            && c.Lat == city.Lat
            && c.Lon == city.Lon
            && c.CountryId == city.CountryId
            && c.Id != city.Id);
        } 
        // POST: api/City
        [HttpPost]
        [Authorize(Roles = "RegistratedUser")]
        public async Task<ActionResult<City>> PostCity(City city)
        {
            if (!_repository.AddInstance(city))
            {
                return NoContent();
            }

            return CreatedAtAction("GetCity", new { id = city.Id }, city);
        }

        // DELETE: api/City/5
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteCity(int id)
        {
            if (!_repository.DeleteInstance(id))
            {
                return NotFound();
            }

            return NoContent();
        }

    }
}

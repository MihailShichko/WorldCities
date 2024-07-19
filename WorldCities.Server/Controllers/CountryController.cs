using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using System.Runtime.CompilerServices;
using WorldCities.Server.Data;
using WorldCities.Server.Models;
using WorldCities.Server.Models.DTO;
using WorldCities.Server.Services.Repository;

namespace WorldCities.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly CoutriesRepository _repository;
        public CountryController(IRepository<Country> repository)
        {
            _repository = (CoutriesRepository)repository;
        }

        [HttpPost]
        [Route("IsDupeField")]
        public async Task<bool> IsDupField(int countryId, string fieldName, string fieldValue)
        {
            var counties = await _repository.GetAll();
            switch (fieldName)
            {
                case "name":
                    return counties.Any(
                        c => c.Name == fieldValue && c.Id != countryId);
                case "iso2":
                    return counties.Any(
                        c => c.ISO2 == fieldValue && c.Id != countryId);
                case "iso3":
                    return counties.Any(
                        c => c.ISO3 == fieldValue && c.Id != countryId);
            }
            
            return false;
        }

        //GET api/Country
        [HttpGet]
        public async Task<ActionResult<ApiResult<CountryDTO>>> GetCountries(int pageIndex = 0, int pageSize = 10,
            string? sortColumn = null, string? sortOrder = null, string? filterColumn = null, string? filterQuery = null)
        {
            var countries = await _repository.GetAll();
            var dto = countries.Select(c => new CountryDTO()//hmm
            {
                Id = c.Id,
                Name = c.Name,
                ISO2 = c.ISO2,
                ISO3 = c.ISO3,
                CityAmount = c.Cities != null ? c.Cities.Count : 0
            }).ToList();
            return await ApiResult<CountryDTO>.CreateAsync(dto, pageIndex, pageSize, sortColumn, sortOrder, filterColumn, filterQuery);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Country>> GetCountry(int id)
        {
            var country = await _repository.GetByIdAsync(id);
            if (country == null)
            {
                return NotFound();
            }

            return country;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutCountry(int id, Country country)
        {
            //if (id != country.Id)
            //{
            //    return BadRequest();
            //}

            if(!_repository.UpdateInstance(country))
            {
                return NotFound();
            }
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCountry(int id)
        {
            if (!_repository.DeleteInstance(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult> PostCountry(Country country)
        {
            if (!_repository.AddInstance(country))
            {
                return NoContent();
            }

            return CreatedAtAction("GetCity", new { id = country.Id }, country);
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using WorldCities.Server.Models;
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

        //GET api/Country
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Country>>> GetCountries()
        {
            var countries = await _repository.GetAll();
            return Ok(countries);
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
            if (id != country.Id)
            {
                return BadRequest();
            }

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

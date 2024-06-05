using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Security;
using WorldCities.Server.Data;
using WorldCities.Server.Models;

namespace WorldCities.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        private IWebHostEnvironment _env;
        public SeedController(ApplicationDbContext dbContext, IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> Import()
        {
            if(!_env.IsDevelopment())
            {
                throw new SecurityException("Not allowed");
            }

            var path = Path.Combine(_env.ContentRootPath, "Source/worldcities.xlsx");
            using var stream = System.IO.File.OpenRead(path);
            using var excelPackage = new ExcelPackage(stream);

            var worksheet = excelPackage.Workbook.Worksheets[0];

            var rows = worksheet.Dimension.End.Row;

            var numberOfCountriesAdded = 0;
            var numberOfCitiesAdded = 0;

            var countriesByName = _dbContext.Countries
             .AsNoTracking()
             .ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
            for(int nRow = 2; nRow < rows; nRow++) 
            {
                var row = worksheet.Cells[nRow, 1, nRow, worksheet.Dimension.End.Column];
                var countryName = row[nRow, 5].GetValue<string>();
                var iso2 = row[nRow, 6].GetValue<string>();
                var iso3 = row[nRow, 7].GetValue<string>();
            
                if(countriesByName.ContainsKey(countryName))
                {
                    continue;
                }

                var country = new Country {Name = countryName, ISO2 = iso2, ISO3 = iso3 };

                await _dbContext.Countries.AddAsync(country);

                countriesByName.Add(countryName, country);
                numberOfCountriesAdded++;
            }

            if(numberOfCountriesAdded > 0)
            {
                await _dbContext.SaveChangesAsync();
            }

            var cities = _dbContext.Cities
                .AsNoTracking()
                .ToDictionary(x => (
                 Name: x.Name,
                 Lat: x.Lat,
                 Lon: x.Lon,
                 CountryId: x.CountryId));
            for(int nRow = 2; nRow < rows; nRow++)
            {
                var row = worksheet.Cells[nRow, 1, nRow, worksheet.Dimension.End.Column];
                var name = row[nRow, 1].GetValue<string>();
                var lat = row[nRow, 3].GetValue<decimal>();
                var lon = row[nRow, 4].GetValue<decimal>();
                var countryName = row[nRow, 5].GetValue<string>();

                var countryId = countriesByName[countryName].Id;

                if (cities.ContainsKey((
                     Name: name,
                     Lat: lat,
                     Lon: lon,
                     CountryId: countryId)))
                    continue;

                var city = new City
                {
                    Name = name,
                    Lat = lat,
                    Lon = lon,
                    CountryId = countryId
                };

                _dbContext.Cities.Add(city);

                numberOfCitiesAdded++;
            }

            if (numberOfCitiesAdded > 0)
                await _dbContext.SaveChangesAsync();

            return new JsonResult(new
            {
                Cities = numberOfCitiesAdded,
                Countries = numberOfCountriesAdded
            });
        }
    }
}

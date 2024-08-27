using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Security;
using WorldCities.Server.Data;
using WorldCities.Server.Models;
using System.IO;


namespace WorldCities.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize(Roles = "Administrator")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IConfiguration _confviguration;

        private IWebHostEnvironment _env;
        public SeedController(ApplicationDbContext dbContext, IWebHostEnvironment env, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration confviguration)
        {
            this._dbContext = dbContext;
            this._env = env;
            this._roleManager = roleManager;
            this._confviguration = confviguration;
            this._userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Import()
        {
            if(!_env.IsDevelopment())
            {
                throw new SecurityException("Not allowed");
            }

            var path = System.IO.Path.Combine(_env.ContentRootPath, "Source/worldcities.xlsx");
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

        [HttpGet]
        public async Task<IActionResult> CreateDefaultUsers()
        {
            var role_RegistratedUser = "RegistratedUser";
            var role_Admin = "Admin";

            if(await _roleManager.FindByNameAsync(role_RegistratedUser) == null)
            {
                await _roleManager.CreateAsync(new IdentityRole(role_RegistratedUser));
            }

            if(await _roleManager.FindByNameAsync(role_Admin) == null)
            {
                await _roleManager.CreateAsync(new IdentityRole(role_Admin));
            }

            var addedUserList = new List<ApplicationUser>();

            var email_admin = "admin@email.com";
            if(await _userManager.FindByNameAsync(email_admin) == null)
            {
                var userAdmin = new ApplicationUser()
                {
                    Email = email_admin,
                    UserName = email_admin,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                await _userManager.CreateAsync(userAdmin, _confviguration["DefaultPasswords:Administrator"]);

                await _userManager.AddToRoleAsync(userAdmin, role_Admin);
                await _userManager.AddToRoleAsync(userAdmin, role_RegistratedUser);

                userAdmin.EmailConfirmed = true;
                userAdmin.LockoutEnabled = false;

                addedUserList.Add(userAdmin);
            }

            var email_User = "user@email.com";

            if (await _userManager.FindByNameAsync(email_User) == null)
            {
                var User = new ApplicationUser()
                {
                    Email = email_User,
                    UserName = email_User,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                await _userManager.CreateAsync(User, _confviguration["DefaultPasswords:RegisteredUser"]);

                await _userManager.AddToRoleAsync(User, role_RegistratedUser);

                User.EmailConfirmed = true;
                User.LockoutEnabled = false;

                addedUserList.Add(User);
            }

            if (addedUserList.Count > 0)
                await _dbContext.SaveChangesAsync();
            return new JsonResult(new
            {
                Count = addedUserList.Count,
                Users = addedUserList
            });
        }
    }
}

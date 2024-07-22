using Microsoft.EntityFrameworkCore;
using WorldCities.Server.Controllers;
using WorldCities.Server.Data;
using WorldCities.Server.Models;
using WorldCities.Server.Services.Repository;

namespace WorldCities.Server.Tests;

public class UnitTest1
{
    [Fact]
    public async Task GetCity()
    {
        //Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("WorldCities")
            .Options;

        using var context = new ApplicationDbContext(options);
        var cityRepository = new CitiesRepository(context);

        cityRepository.AddInstance(new City()
        { 
            Id = 1,
            CountryId = 1,
            Lat = 1,
            Lon = 1,
            Name = "TestCity1"
        });

        context.SaveChanges();

        var controller = new CityController(cityRepository);

        City? cityExisting = null;
        City? cityNonExisting = null;
        
        //Act
        cityExisting = (await controller.GetCity(1)).Value;
        cityNonExisting = (await controller.GetCity(2)).Value;

        //Assert
        Assert.Null(cityNonExisting);
        Assert.NotNull(cityExisting);
    }
}
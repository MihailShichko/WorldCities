using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldCities.Server.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using WorldCities.Server.Controllers;
using WorldCities.Server.Models;

namespace WorldCities.Server.Tests
{
    public class SeedControllerTests
    {
        [Fact]
        public async Task CreateDefaultUser()
        {
            //Arrange
            var options = new
                 DbContextOptionsBuilder<ApplicationDbContext>()
                 .UseInMemoryDatabase(databaseName: "WorldCities")
                 .Options;
            var mockEnv = Mock.Of<IWebHostEnvironment>();

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.SetupGet(x => x[It.Is<string>(s => s ==
                "DefaultPasswords:RegisteredUser")]).Returns("M0ckP$$word");
            mockConfiguration.SetupGet(x => x[It.Is<string>(s => s ==
                "DefaultPasswords:Administrator")]).Returns("M0ckP$$word");

            using var dbContext = new ApplicationDbContext(options);
            var roleManager = IdentityHelper.GetRoleManager(new RoleStore<IdentityRole>(dbContext));
            var userManager = IdentityHelper.GetUserManager(new UserStore<ApplicationUser>(dbContext));

            var controller = new SeedController(dbContext, mockEnv, userManager, roleManager, mockConfiguration.Object);

            ApplicationUser user_Admin = null!;
            ApplicationUser user_User = null!;
            ApplicationUser user_NotExisting = null!;
            
            //Act
            await controller.CreateDefaultUsers();

            user_Admin = await userManager.FindByEmailAsync(
            "admin@email.com");
            user_User = await userManager.FindByEmailAsync(
            "user@email.com");
            user_NotExisting = await userManager.FindByEmailAsync(
            "notexisting@email.com");
            
            //Assert
            Assert.NotNull(user_Admin);
            Assert.NotNull(user_User);
            Assert.Null(user_NotExisting);
        }
    }
}

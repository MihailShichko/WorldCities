
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using WorldCities.Server.Data;
using WorldCities.Server.Models;
using WorldCities.Server.Services.Repository;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;


namespace WorldCities.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            builder.Services.AddControllers();

            builder.Host.UseSerilog((ctx, lc) =>
            {
                lc.ReadFrom.Configuration(ctx.Configuration)
                    .WriteTo.MSSqlServer(connectionString:
                    ctx.Configuration.GetConnectionString("DefaultConnection"),
                    restrictedToMinimumLevel: LogEventLevel.Information,
                    sinkOptions: new MSSqlServerSinkOptions
                    {
                        TableName = "LogEvents",
                        AutoCreateSqlTable = true,
                    })
                    .WriteTo.Console();
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<IRepository<City>, CitiesRepository>();
            builder.Services.AddScoped<IRepository<Country>, CoutriesRepository>();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            }); 
            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseSerilogRequestLogging();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}

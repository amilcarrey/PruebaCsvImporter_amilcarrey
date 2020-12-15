using CsvImporter.Core.Interfaces;
using CsvImporter.Core.Services;
using CsvImporter.Infraestructure.Data;
using CsvImporter.Infraestructure.Data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;

namespace CsvImporter.Application
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = ConfigureServices();

            var serviceProvider = services.BuildServiceProvider();

            // calls the Run method in App, which is replacing Main
            serviceProvider.GetService<App>().Run();
        }
        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            var config = LoadConfiguration();
            services.AddSingleton(config);

            //Adding Logging
            services.AddLogging(loggerBuilder =>
            {
                loggerBuilder.ClearProviders();
                loggerBuilder.AddConsole();
            });

            //database connection
            services.AddDbContext<AcmeContext>(
                options => options.UseSqlServer(config.GetConnectionString("AcmeCorporationConnection"))
            );

            //services
            services.AddScoped<IStockRepository, StockRepository>();            
            services.AddSingleton<ICsvFromUrl, CsvFromUrl>();
            services.AddScoped<IStockService, StockService>();

            // app
            services.AddTransient<App>();

            return services;
        }

        public static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            return builder.Build();
        }
    }
}

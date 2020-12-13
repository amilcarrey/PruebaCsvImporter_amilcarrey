using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CsvImporter.Application
{
    public class App
    {
        private readonly IConfiguration _config;
        private ILogger<App> _logger;
        public App(IConfiguration config, ILogger<App> logger)
        {
            _config = config;
            _logger = logger;
        }
        public void Run()
        {
            var appString = _config.GetValue<string>("AppString");
            Console.WriteLine(appString);

            _logger.LogInformation("PROBANDO EL LOGUEO");

            Console.ReadLine();
        }
    }
}

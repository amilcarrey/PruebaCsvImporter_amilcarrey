using CsvImporter.Core.Services;
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
        private IStockService _stockService;
        public App(IConfiguration config, ILogger<App> logger, IStockService stockService)
        {
            _config = config;
            _logger = logger;
            _stockService = stockService;
        }
        public void Run()
        {
            _stockService.UpdateStockFromCsvAsync();

            Console.ReadLine();
        }
    }
}

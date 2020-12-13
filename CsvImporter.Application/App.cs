using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace CsvImporter.Application
{
    public class App
    {
        private readonly IConfiguration _config;
        public App(IConfiguration config)
        {
            _config = config;
        }
        public void Run()
        {
            var appString = _config.GetValue<string>("AppString");
            Console.WriteLine(appString);

            var lala = _config.GetConnectionString("AcmeCorporationConnection");
            Console.WriteLine(lala);
            Console.ReadLine();
        }
    }
}

using CsvHelper;
using CsvHelper.Configuration;
using CsvImporter.Core.Entities;
using CsvImporter.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CsvImporter.Core.Services
{
    public class StockService : IStockService
    {
        private readonly IStockRepository _stockRepository;
        private readonly ICsvFromUrl _csvStream;
        private ILogger<StockService> _logger;
        public StockService(IStockRepository stockRepository, ICsvFromUrl csvStream, ILogger<StockService> logger)
        {
            _stockRepository = stockRepository;
            _csvStream = csvStream;
            _logger = logger;
        }
        public async Task UpdateStockFromCsvAsync()
        {
            ClearStock();

            Stream response = _csvStream.GetCSVStream("https://storage10082020.blob.core.windows.net/y9ne9ilzmfld/Stock.CSV");


            await AddFromCsvStreamAsync(response);
            //AddFromCsvStreamParallelWay(response);

        }
        public void AddFromCsvStream(Stream csvStream)
        { }
        public async Task AddFromCsvStreamAsync(Stream csvStream)
        {
            Stopwatch timeMeasure = new Stopwatch();
            timeMeasure.Start();

            _logger.LogInformation("We start the magic");

            List<StockModel> listRecords = new List<StockModel>();
            int counter = 0;
            int limit = 500000;

            //using (var reader = new StreamReader(@"C:\test.csv"))
            using (var reader = new StreamReader(@"C:\bigtest.csv"))
            //using (var reader = new StreamReader(csvStream))
            using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture, true))
            {
                csv.Configuration.Delimiter = ";";
                foreach (var record in csv.GetRecords<StockModel>())
                {
                    if (counter == limit || (reader.EndOfStream && counter < limit))
                    {
                        await _stockRepository.CreateBulkAsync(listRecords);
                        counter = 0;
                        listRecords.Clear();
                    }

                    counter++;
                    listRecords.Add(record);
                    //Console.Clear();
                    Console.WriteLine(counter);
                }

                timeMeasure.Stop();
                Console.WriteLine($"Tiempo: {timeMeasure.Elapsed.ToString("mm\\:ss")} ms");
                Console.WriteLine("TERMINO!");
                Console.ReadLine();
            }
        }

        public void AddFromCsvStreamParallelWay(Stream csvStream)
        {
            _logger.LogInformation("We start the magic");

            Stopwatch timeMeasure = new Stopwatch();
            timeMeasure.Start();

            List<StockModel> listRecords = new List<StockModel>();

            //using (var reader = new StreamReader(@"C:\test.csv"))
            using (var reader = new StreamReader(@"C:\bigtest.csv"))
            //using (var reader = new StreamReader(csvStream))
            using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture, true))
            {
                csv.Configuration.Delimiter = ";";

                listRecords = csv.GetRecords<StockModel>().AsParallel().ToList();

                _stockRepository.CreateBulkAsync(listRecords);

                timeMeasure.Stop();

                Console.Clear();
                Console.WriteLine(listRecords.Count);
                Console.WriteLine($"Tiempo: {timeMeasure.Elapsed.ToString("mm\\:ss")} m");
                Console.WriteLine("TERMINO!");
                Console.ReadLine();
            }
        }
        public void ClearStock()
        {
            _stockRepository.Clear();
            Console.WriteLine("CLEAR STOCK");
            //Console.ReadLine();
        }
    }
}

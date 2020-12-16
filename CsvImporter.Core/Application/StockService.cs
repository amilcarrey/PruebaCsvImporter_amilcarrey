﻿using CsvHelper;
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

namespace CsvImporter.Core.Application
{
    public class StockService : IStockService
    {
        private readonly IStockRepository _stockRepository;
        private readonly ICsvFromUrl _csvStream;
        private ILogger<StockService> _logger;
        private readonly IBulk _bulkCreate;
        public StockService(IStockRepository stockRepository, ICsvFromUrl csvStream, ILogger<StockService> logger, IBulk bulkCreate)
        {
            _stockRepository = stockRepository;
            _csvStream = csvStream;
            _logger = logger;
            _bulkCreate = bulkCreate;
        }
        public async Task UpdateStockFromCsvAsync()
        {
            ClearStock();

            Stream response = _csvStream.GetCSVStream("https://storage10082020.blob.core.windows.net/y9ne9ilzmfld/Stock.CSV");
            
            AddFromCsvStreamSqlCopyWay(response);            
        }
        
        /// <summary>
        /// Guarda todos los registros de un csv, realizando varios bulk para optimizar memoria. No utiliza EF Core. 
        /// </summary>
        /// <param name="csvStream">Stream de datos</param>
        public void AddFromCsvStreamSqlCopyWay(Stream csvStream)
        {
            Stopwatch timeMeasure = new Stopwatch();
            timeMeasure.Start();

            _logger.LogInformation("We start the magic");

            List<StockModel> listRecords = new List<StockModel>();
            int counter = 0;
            int limit = 50000;
                        
            using (var reader = new StreamReader(csvStream))
            using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture, true))
            {
                csv.Configuration.Delimiter = ";";

                foreach (var record in csv.GetRecords<StockModel>())
                {
                    if (counter == limit || (reader.EndOfStream && counter < limit))
                    {
                        _bulkCreate.CreateBulk(listRecords);
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
        /// <summary>
        /// Guarda todos los registros de un csv utilizando paralelismo pero sin usar EF Core. La memoria crece exponencialmente según el numero de registros.         
        /// </summary>
        /// <param name="csvStream">Stream de datos</param>
        public void AddFromCsvStreamParallelWaySqlCopy(Stream csvStream)
        {
            _logger.LogInformation("We start the magic");

            Stopwatch timeMeasure = new Stopwatch();
            timeMeasure.Start();

            List<StockModel> listRecords = new List<StockModel>();

            using (var reader = new StreamReader(csvStream))
            using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture, true))
            {
                csv.Configuration.Delimiter = ";";

                listRecords = csv.GetRecords<StockModel>().AsParallel().ToList();

                _bulkCreate.CreateBulk(listRecords);

                timeMeasure.Stop();

                Console.Clear();
                Console.WriteLine(listRecords.Count);
                Console.WriteLine($"Tiempo: {timeMeasure.Elapsed.ToString("mm\\:ss")} m");
                Console.WriteLine("TERMINO!");
                Console.ReadLine();
            }
        }
        /// <summary>
        /// Guarda todos los registros de un csv, realizando varios bulk para optimizar memoria. Utiliza EF Core.
        /// </summary>
        /// <param name="csvStream">Stream de datos</param>
        public async Task AddFromCsvStreamAsync(Stream csvStream)
        {
            Stopwatch timeMeasure = new Stopwatch();
            timeMeasure.Start();

            _logger.LogInformation("We start the magic");

            List<StockModel> listRecords = new List<StockModel>();
            int counter = 0;
            int limit = 500000;

            using (var reader = new StreamReader(csvStream))
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
        /// <summary>
        /// Guarda todos los registros de un csv utilizando paralelismo Utiliza EF Core. La memoria crece exponencialmente según el numero de registros.         
        /// </summary>
        /// <param name="csvStream"></param>
        /// <returns></returns>
        public async Task AddFromCsvStreamParallelWay(Stream csvStream)
        {
            _logger.LogInformation("We start the magic");

            Stopwatch timeMeasure = new Stopwatch();
            timeMeasure.Start();

            List<StockModel> listRecords = new List<StockModel>();

            //using (var reader = new StreamReader(@"C:\test.csv"))
            //using (var reader = new StreamReader(@"C:\bigtest.csv"))
            using (var reader = new StreamReader(csvStream))
            using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture, true))
            {
                csv.Configuration.Delimiter = ";";

                listRecords = csv.GetRecords<StockModel>().AsParallel().ToList();

                await _stockRepository.CreateBulkAsync(listRecords);

                timeMeasure.Stop();

                Console.Clear();
                Console.WriteLine(listRecords.Count);
                Console.WriteLine($"Tiempo: {timeMeasure.Elapsed.ToString("mm\\:ss")} m");
                Console.WriteLine("TERMINO!");
                Console.ReadLine();
            }
        }
        
        /// <summary>
        /// Limpia la base de datos
        /// </summary>
        public void ClearStock()
        {
            _stockRepository.Clear();
            Console.WriteLine("CLEAR STOCK");
            //Console.ReadLine();
        }
    }
}

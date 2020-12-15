using CsvHelper;
using CsvHelper.Configuration;
using CsvImporter.Core.Entities;
using CsvImporter.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CsvImporter.Core.Services
{
    public class StockService : IStockService
    {
        private readonly IStockRepository _stockRepository;
        private readonly ICsvFromUrl _csvStream;
        public StockService(IStockRepository stockRepository, ICsvFromUrl csvStream)
        {
            _stockRepository = stockRepository;
            _csvStream = csvStream;
        }
        public async Task UpdateStockFromCsvAsync()
        {
            ClearStock();

            Stream response = _csvStream.GetCSVStream("https://storage10082020.blob.core.windows.net/y9ne9ilzmfld/Stock.CSV");

            await AddFromCsvStream(response);
        }
        public async Task AddFromCsvStream(Stream csvStream)
        {
            int counter = 0;
            List<StockModel> listRecords = new List<StockModel>();
            //using (var reader = new StreamReader(@"C:\test.csv"))
            //using (var reader = new StreamReader(@"C:\bigtest.csv"))
            using (var reader = new StreamReader(csvStream))
            using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture, true))
            {
                csv.Configuration.Delimiter = ";";
                foreach (var record in csv.GetRecords<StockModel>())
                {
                    counter++;
                    listRecords.Add(record);
                    if (counter == 10000)
                    {
                        await _stockRepository.CreateAsync(listRecords);
                        counter = 0;
                        listRecords.Clear();
                    }
                    Console.WriteLine(counter);
                }

                Console.WriteLine("TERMINO!");
            }            
        }

        public void Save()
        {
            _stockRepository.SaveChangeAsync();
        }
        public void ClearStock()
        {
            _stockRepository.Clear();
            Console.WriteLine("CLEAR STOCK");
            Console.ReadLine();
        }
    }
}

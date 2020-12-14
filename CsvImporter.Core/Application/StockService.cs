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
        public void UpdateStockFromCsv()
        {
            Stream response = _csvStream.GetCSVStream("https://storage10082020.blob.core.windows.net/y9ne9ilzmfld/Stock.CSV");

            using (var reader = new StreamReader(@"C:\test.csv"))
            //using (var reader = new StreamReader(response))
            using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture, true))
            {
                csv.Configuration.Delimiter = ";";
                foreach (var record in csv.GetRecords<StockModel>())
                {
                    AddStock(record);
                    Console.WriteLine(record.ToString());
                }
            }



        }
        public void AddStock(StockModel stock)
        {
            _stockRepository.Create(stock);
        }

        public void ClearStock()
        {
            _stockRepository.Clear();
        }
    }
}

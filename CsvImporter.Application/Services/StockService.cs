using CsvImporter.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CsvImporter.Application.Services
{
    public class StockService : IStockService
    {
        private readonly IStockRepository _stockRepository;
        public StockService(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }
        public void UpdateStockFromCsv()
        {
            throw new NotImplementedException();
        }
    }
}

using CsvImporter.Core;
using CsvImporter.Core.Entities;
using CsvImporter.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CsvImporter.Infraestructure.Data
{
    public class StockRepository : IStockRepository
    {
        public readonly AcmeContext _context;
        public StockRepository(AcmeContext context)
        {
            _context = context;
        }
        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void Create(Stock stock)
        {
            _context.Stock.Add(stock);
            _context.SaveChanges();
        }
    }
}

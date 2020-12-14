using System;
using System.Collections.Generic;
using System.Text;

namespace CsvImporter.Core.Services
{
    public interface IStockService
    {
        public void UpdateStockFromCsv();
        public void AddStock(Entities.StockModel stock);
        public void ClearStock();
    }
}

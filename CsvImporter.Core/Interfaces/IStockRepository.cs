using CsvImporter.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CsvImporter.Core.Interfaces
{
    public interface IStockRepository
    {
        public void Create(StockModel stock);
        public void Clear();
    }
}

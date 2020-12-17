using CsvImporter.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CsvImporter.Core.Interfaces
{
    public interface IStockRepository
    {
        public void CreateBulk(List<StockModel> stock);
        public Task CreateBulkAsync(List<StockModel> stock);
        public Task<bool> Clear();        
    }
}

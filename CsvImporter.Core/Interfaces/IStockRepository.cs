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
        public void Create(StockModel stock);
        public Task CreateAsync(List<StockModel> stock);
        public void Clear();
        public Task SaveChangeAsync();
    }
}

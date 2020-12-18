using CsvImporter.Core.Entities;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CsvImporter.Core.Application
{
    public interface IStockService
    {
        public void UpdateStockFromCsvAsync();
        public Task AddBySqlCopyAsync(Stream csvStream);
        public void AddBySqlCopy(Stream csvStream);
        public Task AddByEfCoreAsync(Stream csvStream);
        public void AddByEfCore(Stream csvStream);
        public Task AddByEfCoreWithParallelismAsync(Stream csvStream);
        public Task CreateBulkAsync(List<StockModel> stockList);
        public Task<Stream> GetStream(string url);
        public Task<bool> ClearStock();
    }
}

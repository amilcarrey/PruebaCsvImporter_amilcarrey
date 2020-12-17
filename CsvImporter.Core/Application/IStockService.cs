using CsvImporter.Core.Entities;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CsvImporter.Core.Application
{
    public interface IStockService
    {
        public void UpdateStockFromCsvAsync();
        public Task AddFromCsvStreamSqlCopyWay(Stream csvStream);
        public Task AddFromCsvStreamParallelWaySqlCopy(Stream csvStream);
        public Task AddFromCsvStreamAsync(Stream csvStream);
        public Task AddFromCsvStreamParallelWay(Stream csvStream);
        public Task CreateBulkAsync(IList<StockModel> stockList);
        public Task<Stream> GetStream(string url);
        public Task<bool> ClearStock();
    }
}

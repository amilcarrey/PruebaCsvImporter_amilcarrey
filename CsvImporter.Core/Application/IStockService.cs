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
        public Task<Stream> GetStream(string url);
        public Task<bool> ClearStock();
    }
}

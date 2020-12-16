using System.IO;
using System.Threading.Tasks;

namespace CsvImporter.Core.Application
{
    public interface IStockService
    {
        public Task UpdateStockFromCsvAsync();
        public void AddFromCsvStreamSqlCopyWay(Stream csvStream);
        public void AddFromCsvStreamParallelWaySqlCopy(Stream csvStream);
        public Task AddFromCsvStreamAsync(Stream csvStream);
        public Task AddFromCsvStreamParallelWay(Stream csvStream);
        public void ClearStock();
    }
}

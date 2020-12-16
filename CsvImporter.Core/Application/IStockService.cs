using System.IO;
using System.Threading.Tasks;

namespace CsvImporter.Core.Services
{
    public interface IStockService
    {
        public Task UpdateStockFromCsvAsync();
        public void AddFromCsvStream(Stream csvStream);
        public Task AddFromCsvStreamAsync(Stream csvStream);
        public void ClearStock();
    }
}

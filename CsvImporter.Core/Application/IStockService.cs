using System.IO;
using System.Threading.Tasks;

namespace CsvImporter.Core.Services
{
    public interface IStockService
    {
        public Task UpdateStockFromCsvAsync();
        public Task AddFromCsvStream(Stream csvStream);
        public void ClearStock();
    }
}

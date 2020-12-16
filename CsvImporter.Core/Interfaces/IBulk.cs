using CsvImporter.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CsvImporter.Core.Interfaces
{
    public interface IBulk
    {
        void CreateBulk(List<StockModel> stock);
        Task CreateBulkAsync(List<StockModel> stock);
    }
}

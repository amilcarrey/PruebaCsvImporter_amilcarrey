using CsvImporter.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CsvImporter.Core.Interfaces
{
    public interface IBulk
    {
        void CreateBulk(List<StockModel> stock);
    }
}

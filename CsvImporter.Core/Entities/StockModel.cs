using CsvHelper.Configuration.Attributes;
using System;

namespace CsvImporter.Core.Entities
{
    public class StockModel
    {
        [Ignore]
        public Guid Id { get; set; } = Guid.NewGuid();
        public int PointOfSale { get; set; }
        public string Product { get; set; }
        public DateTime Date { get; set; }
        public int Stock { get; set; }
    }
}

using System;

namespace CsvImporter.Core.Entities
{
    public class StockModel
    {
        public Guid Id { get; set; }
        public int PointOfSale { get; set; }
        public string Product { get; set; }
        public DateTime Date { get; set; }
        public int Stock { get; set; }
    }
}

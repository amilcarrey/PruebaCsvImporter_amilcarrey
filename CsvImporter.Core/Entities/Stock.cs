using System;

namespace CsvImporter.Core.Entities
{
    public class Stock
    {
        public int PointOfSale { get; set; }
        public long Product { get; set; }
        public DateTime Date { get; set; }
        public int Qty { get; set; }
    }
}

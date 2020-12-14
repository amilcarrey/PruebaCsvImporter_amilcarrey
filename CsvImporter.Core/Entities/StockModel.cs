using CsvHelper.Configuration.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace CsvImporter.Core.Entities
{
    public class StockModel
    {
        [Key]
        [Ignore]
        public Guid Id { get; set; } = new Guid();
        public int PointOfSale { get; set; }
        public string Product { get; set; }
        public DateTime Date { get; set; }
        public int Stock { get; set; }
    }
}

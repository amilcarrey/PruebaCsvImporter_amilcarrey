using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using CsvImporter.Core.Entities;

namespace CsvImporter.Infraestructure.Data 
{ 
    public class AcmeContext : DbContext
    {
        public AcmeContext(DbContextOptions<AcmeContext> options) : base(options) { }
        public DbSet<Stock> Stock { get; set; }

    }
}

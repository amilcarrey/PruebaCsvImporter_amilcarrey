using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using CsvImporter.Core.Entities;

namespace CsvImporter.Infraestructure.Data 
{ 
    public class AcmeContext : DbContext
    {
        //private const string connectionString = "Server=localhost\\SQLEXPRESS;Database=AcmeCorporation;Trusted_Connection=True;MultipleActiveResultSets=true;";

        public AcmeContext(DbContextOptions<AcmeContext> options) : base(options) { }
        public DbSet<StockModel> Stock { get; set; }


        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(connectionString);
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Property Configurations
            modelBuilder.Entity<StockModel>()
                .Property(p => p.Id)
                ;
        }

    }
}

using CsvImporter.Core.Entities;
using CsvImporter.Core.Interfaces;
using FastMember;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace CsvImporter.Infraestructure.Data.Services
{
    public class BulkUploadToSql : IBulk
    {
        private readonly IConfiguration _configuration;
        private ILogger<BulkUploadToSql> _logger;
        public BulkUploadToSql(IConfiguration configuration, ILogger<BulkUploadToSql> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        
        public void CreateBulk(List<StockModel> stock)
        {
            using (SqlConnection Connection = new SqlConnection(_configuration.GetConnectionString("AcmeCorporationConnection")))
            {
                Connection.Open();
                _logger.LogInformation("Connection Open");
              

                _logger.LogInformation("Begin Bulk!");
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(Connection))
                using (var reader = ObjectReader.Create(stock, "Id", "PointOfSale", "Product", "Date", "Stock"))
                {
                    bulkCopy.DestinationTableName = "Stock";

                    try
                    {
                        // Write from the source to the destination.
                        bulkCopy.WriteToServer(reader);
                        _logger.LogInformation("Write Succesfull");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Write Fail");
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
    }
}

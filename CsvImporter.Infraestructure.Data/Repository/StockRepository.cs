using CsvImporter.Core.Entities;
using CsvImporter.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CsvImporter.Infraestructure.Data
{
    public class StockRepository : IStockRepository
    {
        public readonly AcmeContext _context;
        private readonly ILogger<StockRepository> _logger;
        public StockRepository(AcmeContext context, ILogger<StockRepository> logger)
        {
            _context = context;
            _context.ChangeTracker.AutoDetectChangesEnabled = false;
            _logger = logger;

        }
        public async Task<bool> Clear()
        {
            try
            {
                int rowAffected = await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE STOCK");
                return rowAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return false;
            }

        }

        public void Create(StockModel stock)
        {
            try
            {
                _context.Stock.Add(stock);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            
        }

        public void CreateBulk(List<StockModel> listStock)
        {
            try
            {
                _context.Stock.BulkInsert(listStock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        public async Task CreateBulkAsync(List<StockModel> listStock)
        {
            try
            {
                await _context.Stock.BulkInsertAsync(listStock);
                DetachAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

        }
        public void DetachAll()
        {
            _context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added ||
                        e.State == EntityState.Modified ||
                        e.State == EntityState.Deleted ||
                        e.State == EntityState.Unchanged)
            .ToList()
            .AsParallel()
            .ForAll(entry => entry.State = EntityState.Detached);
        }


    }
}

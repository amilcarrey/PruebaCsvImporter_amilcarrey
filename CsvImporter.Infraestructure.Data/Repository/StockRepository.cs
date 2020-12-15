using CsvImporter.Core;
using CsvImporter.Core.Entities;
using CsvImporter.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsvImporter.Infraestructure.Data
{
    public class StockRepository : IStockRepository
    {
        public readonly AcmeContext _context;
        public StockRepository(AcmeContext context)
        {
            _context = context;
            _context.ChangeTracker.AutoDetectChangesEnabled = false;            
        }
        public void Clear()
        {
            _context.Database.ExecuteSqlRaw("TRUNCATE TABLE STOCK");
            _context.SaveChanges();
        }

        public void Create(StockModel stock)
        {
            _context.Stock.Add(stock);
            _context.SaveChanges();
        }

        public async System.Threading.Tasks.Task CreateAsync(List<StockModel> listStock)
        {
            await _context.Stock.BulkInsertAsync(listStock);
            DetachAll();
        }

        public async System.Threading.Tasks.Task SaveChangeAsync()
        {
            await _context.SaveChangesAsync();            
        }

        public void DetachAll()
        {
            var changedEntriesCopy = _context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added ||
                        e.State == EntityState.Modified ||
                        e.State == EntityState.Deleted ||
                        e.State == EntityState.Unchanged)
            .ToList();

            foreach (var entry in changedEntriesCopy)
                entry.State = EntityState.Detached;
        }
        
    }
}

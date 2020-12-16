using CsvImporter.Core.Entities;
using CsvImporter.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public void CreateBulk(List<StockModel> listStock)
        {
            _context.Stock.BulkInsert(listStock);
        }

        public async Task CreateBulkAsync(List<StockModel> listStock)
        {             
            await _context.Stock.BulkInsertAsync(listStock);
            DetachAll();
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

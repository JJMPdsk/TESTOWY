using System.Threading.Tasks;
using Data.Models;

namespace Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        // przykład:
        // private IDaysRepository _daysRepository;
        // public IDaysRepository => _daysRepository ?? (_daysRepository = new DaysRepository(_context));

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }
        
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }


        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
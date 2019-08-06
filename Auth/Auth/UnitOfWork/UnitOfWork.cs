using Auth.Models;
using Auth.Repository.Interfaces;

namespace Auth.UnitOfWork
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

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
using System.Threading.Tasks;
using Data.Models;
using Data.Repositories;
using Data.Repositories.Interfaces;

namespace Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        private IUsersRepository _usersRepository;
        public IUsersRepository UsersRepository =>
            _usersRepository ?? (_usersRepository = new UsersRepository(_context));

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
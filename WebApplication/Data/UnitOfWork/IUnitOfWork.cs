using System;
using System.Threading.Tasks;
using Data.Repositories.Interfaces;

namespace Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUsersRepository UsersRepository { get; }

        int Complete();
        Task<int> CompleteAsync();
    }
}
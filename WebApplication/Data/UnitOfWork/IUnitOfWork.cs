using System;
using System.Threading.Tasks;

namespace Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        // przykład: IDayRepository Days { get; }
        int Complete();
        Task<int> CompleteAsync();
    }
}
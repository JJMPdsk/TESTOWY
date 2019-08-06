using System;

namespace Auth.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        // przykład: IDayRepository Days { get; }
        int Complete();
    }
}
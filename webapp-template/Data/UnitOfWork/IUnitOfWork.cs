using System;

namespace Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        // przykład: IDayRepository Days { get; }
        int Complete();

    }
}
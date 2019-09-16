using System.Threading.Tasks;
using Data.UnitOfWork;

namespace Core.Services.Interfaces
{
    public interface IService
    {
        /// <summary>
        /// Aktualizuje bazę danych, a w przypadku wyjątku zwraca wiadomość zwrotną
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        string UpdateDatabase(IUnitOfWork unitOfWork);
        
        Task<string> UpdateDatabaseAsync(IUnitOfWork unitOfWork);
    }
}
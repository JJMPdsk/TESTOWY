using System.Threading.Tasks;
using Core.Services.Interfaces;
using Data.UnitOfWork;

namespace Core.Services
{
    public class Service : IService
    {
        public string UpdateDatabase(IUnitOfWork unitOfWork)
        {

            try
            {
                unitOfWork.Complete();
            }
            catch
            {
                return "Błąd podczas aktualizacji bazy!";
            }
            return string.Empty;
        }
        
        
        public async Task<string> UpdateDatabaseAsync(IUnitOfWork unitOfWork)
        {

            try
            {
                await unitOfWork.CompleteAsync();
            }
            catch
            {
                return "Błąd podczas aktualizacji bazy!";
            }
            return string.Empty;
        }
    }
    
    
}
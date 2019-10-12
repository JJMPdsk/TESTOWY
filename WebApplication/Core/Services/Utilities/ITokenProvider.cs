using System.Threading.Tasks;

namespace Core.Services.Utilities
{
    public interface ITokenProvider
    {
        /// <summary>
        ///     Pobiera i zwraca token przypisany do użytkownika
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<string> GetCurrentTokenAsync(string userName);
    }
}
using System;
using System.Threading.Tasks;
using Data.UnitOfWork;

namespace Core.Services.Utilities
{
    public class TokenProvider : ITokenProvider, IDisposable
    {
        private readonly IUnitOfWork _unitOfWork;

        public TokenProvider(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Dispose()
        {
            _unitOfWork?.Dispose();
        }

        public async Task<string> GetCurrentTokenAsync(string userName)
        {
            return await _unitOfWork.UsersRepository.GetCurrentTokenAsync(userName);
        }
    }
}
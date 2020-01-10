using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Data.Models;
using Data.Repositories.Interfaces;

namespace Data.Repositories
{
    public class UsersRepository : Repository<ApplicationUser>, IUsersRepository
    {
        public UsersRepository(DbContext context) : base(context)
        {
        }

        public async Task<bool> AssignTokenToUserAsync(string userName, string token)
        {
            var user = await Context.Set<ApplicationUser>().SingleOrDefaultAsync(c => c.UserName == userName);
            if (user == null) return false;
            user.Token = token;
            return true;
        }

        public async Task<string> GetCurrentTokenAsync(string userName)
        {
            var user = await GetUserByUserNameAsync(userName);
            return user.Token;
        }

        public async Task<ApplicationUser> GetUserByUserNameAsync(string userName)
        {
            return await Context.Set<ApplicationUser>().SingleOrDefaultAsync(c => c.UserName == userName);
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            return await Context.Set<ApplicationUser>().SingleOrDefaultAsync(c => c.Email == email);

        }

        public async Task<bool> ChangeUserFirstNameClaimAsync(string userId, string oldName, string newName)
        {
            var user = await Context.Set<ApplicationUser>().SingleOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;

            var claim = user.Claims.SingleOrDefault(c => c.UserId == userId && c.ClaimValue.Equals(oldName) &&  c.ClaimType.Equals("FirstName"));
            if (claim == null) return false;

            claim.ClaimValue = newName;
            return await Context.SaveChangesAsync() > 0;
        }
    }
}
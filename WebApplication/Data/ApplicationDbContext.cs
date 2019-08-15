using Data.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // przykład dodania tabeli z bazy danych
        // public virtual DbSet<Day> Days { get; set; }
        public ApplicationDbContext()
            : base("DefaultConnection", false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}
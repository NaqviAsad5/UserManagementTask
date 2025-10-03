using Microsoft.EntityFrameworkCore;
using UserManagement.Models.DbModels;

namespace UserManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        //DBSet
        public DbSet<tblUser> tblUser { get; set; }
    }
}

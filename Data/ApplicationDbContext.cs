using LoginApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LoginApi.Data
{

    public class ApplicationDbContext :DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        //Tables
        public DbSet<User> Users{get;set;}
    }
}
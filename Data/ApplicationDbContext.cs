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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Make email unique
            modelBuilder.Entity<User>()
            .HasIndex(u=> u.Email)
            .IsUnique();


            //Make contact number unique
            modelBuilder.Entity<User>()
            .HasIndex(u=> u.ContactNumber)
            .IsUnique();

          

        }
    }
}
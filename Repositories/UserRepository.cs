using System.Collections;
using System.Runtime.CompilerServices;
using LoginApi.Data;
using LoginApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;


namespace LoginApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        //readonly
        private readonly ApplicationDbContext _context;

        private readonly string _connectionString;

        public UserRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;

            _connectionString = configuration.GetConnectionString("AuthConnection") 
            ?? throw new InvalidOperationException("Connection String 'AuthConnection' cannot be found");
        }

       //Get user by id
       public async Task<User?> GetbyId(int id)
        {
           return await _context.Users.FindAsync(id);
    
        }

        //update user details
        public async Task UpdateDetails(User user)
        {
             _context.Users.Update(user);
             await _context.SaveChangesAsync();
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users
            .FirstOrDefaultAsync(e=> e.Email == email);
            
        }


    }


}
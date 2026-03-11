using LoginApi.Data;
using LoginApi.Models;
using Microsoft.EntityFrameworkCore;


namespace LoginApi.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        //readonly
        private readonly ApplicationDbContext _context;

        public AuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //Signup

          public async Task RegisterUser (User user)
        {
            _context.Users.Add(user);
           await _context.SaveChangesAsync();

        }

        public async Task<User?> CheckByEmail(string email)
        {
            return await _context.Users
            .FirstOrDefaultAsync(e=> e.Email == email);
        }

        public async Task UpdateUser(User user)
        {
              _context.Users.Update(user);
              await _context.SaveChangesAsync();

        }


       public async Task<User?> GetByResetToken (string token)
        {
            return await _context.Users
            .FirstOrDefaultAsync(u => u.ResetToken == token);
        }

    }


}
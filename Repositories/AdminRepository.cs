using System.Collections;
using System.Runtime.CompilerServices;
using LoginApi.Data;
using LoginApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;


namespace LoginApi.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        //readonly
        private readonly ApplicationDbContext _context;

        public AdminRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //List of all Users in database
        public async Task<IEnumerable<UserDto>> ListAllUsers()
        {
            return await _context.Users
                        .Select(u => new UserDto
                        {
                            FullName = u.FullName,
                            Email = u.Email,
                            Role = u.Role,
                        })
                        .ToListAsync();
            
        }


    }


}
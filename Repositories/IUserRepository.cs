using System.Collections;
using LoginApi.Models;

namespace LoginApi.Repositories
{
    public interface IUserRepository
    {
      
      //Get user details by id
      Task<User?> GetbyId(int id);

      Task UpdateDetails(User user);

      Task<User?> GetUserByEmail(string email);
       

        
    }

}
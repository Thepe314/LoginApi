using System.Collections;
using LoginApi.Models;

namespace LoginApi.Repositories
{
    public interface IAdminRepository
    {
       //List of all users on database
       Task<IEnumerable<UserDto>> ListAllUsers();

        
    }

}
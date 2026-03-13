using LoginApi.Models;

namespace LoginApi.Repositories
{
    public interface IAuthRepository
    {
        //signup
        Task RegisterUser (User user);

        //login
        Task<User?>LoginUser(User user);

        //Check By email
        Task<User?> CheckByEmail(string email);

        //Update user
        Task UpdateUser(User user);


        Task<User?> GetByResetToken (string token);

        
    }

}
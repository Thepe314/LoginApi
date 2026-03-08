using LoginApi.Models;

namespace LoginApi.Repository
{
    public interface IAuthRepository
    {
        //signup
        Task RegisterUser (User user);

        //login
        Task LoginUser(User user);

        
    }

}
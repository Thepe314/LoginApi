using System.ComponentModel.DataAnnotations;

namespace LoginApi.Models
{
    public class UserUpdateDto
    {
  
        public string? FullName{get;set;}
        
        [EmailAddress]
        public string? Email{get;set;}

        public string? OldPassword{get;set;}
        public  string? NewPassword{get;set;}

         
    }
}
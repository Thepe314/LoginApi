using System.ComponentModel.DataAnnotations;

namespace LoginApi.Models
{
    public class UserDto
    {

        [Required(ErrorMessage ="Full Name Required")]
        public required string FullName{get;set;}

        [EmailAddress(ErrorMessage ="Email is invalid format")]
        public required string Email{get;set;}

         public required string Role { get; set; } 
    }
}
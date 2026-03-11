using System.ComponentModel.DataAnnotations;

namespace LoginApi.Models
{
    public class  ResetPasswordDto
    {

        [EmailAddress(ErrorMessage = "Invalid Email Format")]
        [Required(ErrorMessage = "Email is required")]
        public required string Email{get;set;}

        [MinLength(8,ErrorMessage = "Password must be at least 8 characters long")]
        [Required(ErrorMessage ="Password is Required")]
        public required string Password{get;set;}

        [Required(ErrorMessage ="Token is Required")]
        public required string Token {get;set;}

    }
}
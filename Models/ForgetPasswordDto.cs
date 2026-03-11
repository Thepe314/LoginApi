using System.ComponentModel.DataAnnotations;

namespace LoginApi.Models
{
    public class  ForgetPasswordDto
    {

        [EmailAddress(ErrorMessage = "Invalid Email Format")]
        [Required(ErrorMessage = "Email is required")]
        public required string Email{get;set;}

    }
}
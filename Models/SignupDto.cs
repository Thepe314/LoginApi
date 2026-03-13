using System.ComponentModel.DataAnnotations;

namespace LoginApi.Models
{
    public class  SignupDto
    { 
        [Required(ErrorMessage = "Full name is required")]
        [MinLength(3)]
        public required string FullName{get;set;}

        [EmailAddress(ErrorMessage = "Invalid Email Format")]
        [Required(ErrorMessage = "Email is required")]
        public required string Email{get;set;}

        [MinLength(8,ErrorMessage = "Password must be at least 8 characters long")]
        [Required(ErrorMessage ="Password is Required")]
        public required string Password{get;set;}


        [Required(ErrorMessage ="Contact Number is Required")]
        public required string ContactNumber {get;set;}

        [Required]
        [MaxLength(200)]
        public required string Address{get;set;}
    }
}
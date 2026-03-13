using System.ComponentModel.DataAnnotations;

namespace LoginApi.Models
{
    public class  SignupDto
    { 
        [Required(ErrorMessage = "Full name is required")]
        [RegularExpression(@"^[A-Za-z\s]$",ErrorMessage = "Only characters allowed")]
        [MinLength(3)]
        public required string FullName{get;set;}

        [EmailAddress(ErrorMessage = "Invalid Email Format")]
        [Required(ErrorMessage = "Email is required")]
        public required string Email{get;set;}

        [MinLength(8,ErrorMessage = "Password must be at least 8 characters long")]
        [Required(ErrorMessage ="Password is Required")]
        public required string Password{get;set;}

        [RegularExpression(@"^\d{10}$",ErrorMessage = "Contact number must be exactly 10 digits")]
        [Required(ErrorMessage ="Contact Number is Required")]
        public required string ContactNumber {get;set;}

        [Required]
        [RegularExpression(@"^[A-Za-z0-9\s,.-]$")]
        [MaxLength(200)]
        public required string Address{get;set;}
    }
}
using System.ComponentModel.DataAnnotations;

namespace LoginApi.Models
{
    public class User
    {
        public int Id {get;set;}

        public required string FullName{get;set;}

        [EmailAddress]
        public required string Email{get;set;}

        public required string Password{get;set;}

        public string? ContactNumber {get;set;}

        public string? Address{get;set;}

         public string Role { get; set; } = "User"; 

         public string? ResetToken {get;set;}

         public DateTime? ResetTokenExpiry {get;set;}

         
    }
}
using System.ComponentModel.DataAnnotations;

namespace LoginApi.Models
{
    public class User
    {
        public int Id {get;set;}

        public required string FullName{get;set;}

        [EmailAddress]
        public required string Email{get;set;}

        [MinLength(6)]
        public required string Password{get;set;}
    }
}
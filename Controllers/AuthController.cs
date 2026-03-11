
using System.Security.Cryptography;
using LoginApi.Models;
using LoginApi.Repositories;
using LoginApi.Service;
using Microsoft.AspNetCore.Mvc;


namespace LoginApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        //readonly
        private readonly IAuthRepository _aRepo;

        //readonly
        private readonly JwtService _jwtService;

        public AuthController(IAuthRepository aRepo, JwtService jwtService)
        {
            _aRepo = aRepo;

            _jwtService = jwtService;
        }

        //Endpoints
        //Creating a new user

        [HttpPost("signup")]
        public async Task<IActionResult> SignupUser(SignupDto dto)
        {
            //Check if email exists
            var email = dto.Email.Trim().ToLower(); //To lowercase
            var existing = await _aRepo.CheckByEmail(email);
            if(existing != null)
            {
                return BadRequest("Email already registered");
            }

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "User"
            };   
            await _aRepo.RegisterUser(user);

            return Ok("Successfully Signed up");
        }
       

        //Login of user
       [HttpPost("login")]
       public async Task<IActionResult> LoginUser(LoginDto dto)
        {
            //Check if user exist or not
            var email = dto.Email.Trim().ToLower(); // make it lowercase
            var existing = await _aRepo.CheckByEmail(email);
            if(existing == null)
            {
                return Unauthorized("Invalid Email or Password");
            }

            //Verify password 
            bool VerifyPassword = BCrypt.Net.BCrypt.Verify(dto.Password, existing.Password);

            if(!VerifyPassword)
            {
                return Unauthorized("Invalid Email or Password");
            }
          
          //Generate a jwt token

          var token =  _jwtService.GenerateToken(existing);

            return Ok(new{Message= "Logged in succesfully",Email = existing.Email , Token = token});
        }

          //Forget Password: Asks for email and generates a reset token
        [HttpPost("Forget-Password")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordDto dto)
        {
            //Check if Email exists
            var user = await _aRepo.CheckByEmail(dto.Email);

            if(user == null)
            {
                return Ok(new{Message = "If email exists, Reset Token will be given"});
            }

            //Generate the reset token
            var resetToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            user.ResetToken = resetToken;
            user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(15);

            await _aRepo.UpdateUser(user);

            return Ok(new{Message = "Here is the refresh token", ResetToken = resetToken});

        }


        //Reset-Password : Using the reset token from Forget-password, user can change their pw
        [HttpPost("Reset-Password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            //find user by token
            var existing = await _aRepo.GetByResetToken(dto.Token);
            if(existing == null || existing.ResetToken != dto.Token || existing.ResetTokenExpiry <DateTime.UtcNow)
            {
                return BadRequest("Invalid or expired reset token");
            }
            
            
               existing.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
               existing.ResetToken = null;
               existing.ResetTokenExpiry = null;

               await _aRepo.UpdateUser(existing);
            

            return Ok(new{Message ="Password reset successfully"});


        }

        

    }
}
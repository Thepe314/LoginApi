
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

        private readonly EncryptionService _encrytionService;

        public AuthController(IAuthRepository aRepo, JwtService jwtService, EncryptionService encryptionService)
        {
            _aRepo = aRepo;

            _jwtService = jwtService;

            _encrytionService = encryptionService;


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
                return Conflict(new{Message = "Email already registered"});
            }

            var user = new User
            {
                FullName = dto.FullName.Trim(),
                Email = email,
                Password = _encrytionService.Encrypt(dto.Password), //Encrypt password
                ContactNumber = dto.ContactNumber.Trim(),
                Address = dto.Address.Trim(),
                Role = "User"
            };   
            await _aRepo.RegisterUser(user);

            return Ok(new{Message="Successfully Signed up"});
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
            bool VerifyPassword = _encrytionService.VerifyPassword(dto.Password,existing.Password);

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
            var user = await _aRepo.CheckByEmail(dto.Email.Trim().ToLower());

            if(user == null)
            {
                return Ok(new{Message = "If email exists, Reset Token will be given"});
            }

            //Generate the reset token
            var resetToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            user.ResetToken = resetToken;
            user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(15);

            await _aRepo.UpdateUser(user);

            return Ok(new{Message = "Reset Token has been sent, Check your database"});

        }


        //Reset-Password : Using the reset token from Forget-password, user can change their pw
        [HttpPost("Reset-Password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            //find user by token
            var existing = await _aRepo.GetByResetToken(dto.Token);
            if(existing == null || existing.ResetToken != dto.Token || existing.ResetTokenExpiry <DateTime.UtcNow)
            {
                return BadRequest(new{Message ="Invalid or expired reset token"});
            }

            bool isSamePassword = _encrytionService.VerifyPassword(dto.Password,existing.Password);
            if(isSamePassword)
            {
                return BadRequest(new{Message="Password cannot be reused"});
            }

               existing.Password = _encrytionService.Encrypt(dto.Password);
               existing.ResetToken = null;
               existing.ResetTokenExpiry = null;

               await _aRepo.UpdateUser(existing);
            

            return Ok(new{Message ="Password reset successfully"});


        }

        

    }
}
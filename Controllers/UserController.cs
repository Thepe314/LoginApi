using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using LoginApi.Models;
using LoginApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace LoginApi.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UserController : ControllerBase
    {
        //readonly
        private readonly IUserRepository _uRepo;

        public UserController(IUserRepository uRepo)
        {
            _uRepo = uRepo;
        }

            //Endpoints
        [HttpGet("Profile")]
        public async Task<IActionResult> Profile()
        {
            //Get userId from jwt first
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new{Message="Invalid token"});
            }

            //Map the jwt to Dto
             var currentUser = new UserDto
            {
                Email = User.FindFirst(u => u.Type == ClaimTypes.Email)?.Value ?? "",
                FullName = User.FindFirst(u => u.Type == "FullName")?.Value ?? "",
                Role = User.FindFirst(u => u.Type == ClaimTypes.Role)?.Value ?? "",
            };

            //Check role
            if(currentUser.Role != "User")
            {
                return StatusCode(403,new{Message="Access Denied. You do not have permission"});
            }


            return Ok(new{Message = "Here is the profile details",currentUser});

        }

        //using a user's id update their profile and only they can do it
        [HttpPatch("Update-Profile")]
        public async Task<IActionResult> UpdateUserProfile([FromBody]UserUpdateDto? dto)
        {

            //First we check if token is there
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if(!int.TryParse(userIdClaim,out int userId))
            {
                return Unauthorized(new{Message="Invalid Token"});
            }

            //Then we check if User is really user
            if(!User.IsInRole("User"))
            {
                return StatusCode(403,"Access Denied, Not User");
            }
            //Check user by id
            var existing = await _uRepo.GetbyId(userId);
            if(existing == null )
            {
                return NotFound("User not found");
            }
            //Update credentials 
            
            if(!string.IsNullOrWhiteSpace(dto?.Email) && dto.Email != existing.Email)
            {
                 //Check if email is unique
                var email = await _uRepo.GetUserByEmail(dto.Email.Trim().ToLower());

                if(email != null && email.Id != userId)
                {
                    return Conflict(new{Message=" Email already exists"});
                }

                 existing.Email = dto.Email.Trim().ToLower();
            }
           
            //Update if Fullname is Provided
            if(!string.IsNullOrEmpty(dto?.FullName))
            {
                 existing.FullName = dto.FullName.Trim();
            }

            //Handle Password update if both fields are provided
            if(!string.IsNullOrEmpty(dto?.OldPassword) && !string.IsNullOrEmpty(dto.NewPassword))
            {
                 //verify password first
                bool isOldPassword = BCrypt.Net.BCrypt.Verify(dto.OldPassword, existing.Password);

                if(!isOldPassword)
                {
                    return Unauthorized("Invalid Password");
                }

                //Hash and update the new password
                existing.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            }
           
            //Save changes
            await _uRepo.UpdateDetails(existing);
            
            //Return these data
            return Ok(new{Message ="Updated Successfully", user = new{existing.Id, existing.Email,existing.FullName}});

            
        }

    }
}
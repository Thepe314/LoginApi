using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using LoginApi.Data;
using LoginApi.Models;
using LoginApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Caching.Memory;


namespace LoginApi.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize]
    public class AdminController : ControllerBase
    {
        //readonly
        private readonly IAdminRepository _AdminRepo;


        public AdminController(IAdminRepository adminRepo)
        {
            _AdminRepo = adminRepo;
          
            
        }

        //Endpoints
        [HttpGet("Get-All-Users")]
        public async Task<IActionResult> ListAllUser()
        {

            //Validated Token
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if(!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Invalid Token");
            } 

            //Map JWT To DTO
            var currentUser = new UserDto
            {
                //User in this is a ClaimsPrincipal, it takes the values from the payload.
                Email = User.FindFirst(c => c.Type == ClaimTypes.Email)?.Value ?? "",
                Role = User.FindFirst(c=>c.Type == ClaimTypes.Role)?.Value ?? "",
                FullName = User.FindFirst(c => c.Type == "FullName")?.Value ?? "",

            };

            //Check the Role
            if(currentUser.Role !="Admin")
            {
                return StatusCode(403,new{Message="Access denied. Admins only"});
            }

           //List the Users
            var user = await _AdminRepo.ListAllUsers();

           return Ok(new{Message = "Here are the listed users:",RequestedBy = currentUser.Email,user});
        }

        // [HttpGet("cache-status")]
        // public IActionResult CacheStatus()
        // {
        //     if (_mCache.TryGetValue("AllUsers", out var cachedUsers))
        //     {
        //         return Ok(new { 
        //             Status = "✅ Cache is ACTIVE",
        //             Data = cachedUsers
        //         });
        //     }
        //     else
        //     {
        //         return Ok(new { 
        //             Status = " Cache is EMPTY - will fetch from DB on next request"
        //         });
        //     }
        // }
        

    }
}
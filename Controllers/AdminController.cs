using System.ComponentModel.DataAnnotations;
using LoginApi.Data;

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


        public AdminController(IAdminRepository adminRepo, IMemoryCache mCache)
        {
            _AdminRepo = adminRepo;
          
            
        }

        //Endpoints
        [HttpGet("Get-All-Users")]
        public async Task<IActionResult> ListAllUser()
        {
            if(!User.IsInRole("Admin"))
            {
                return StatusCode(403,"Access denied. Admins only");
            }

           
            var user = await _AdminRepo.ListAllUsers();

           return Ok(new{Message = "Here are the listed users:",user});
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
        //             Status = "❌ Cache is EMPTY - will fetch from DB on next request"
        //         });
        //     }
        // }
        

    }
}
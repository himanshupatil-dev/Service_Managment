using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Service_Managment.Application.DTOs;
using Service_Managment.Application.Interface;

namespace Service_Managment.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _authService;
       
        public UserController(IUserService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("register/user")]
        public async Task<IActionResult> RegisterUser(RegisterUserRequest request)
        {
            var result = await _authService.RegisterUserAsync(request);

            return Ok(result);
        }

        
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service_Managment.Application.DTOs;
using Service_Managment.Application.Interface;

namespace Service_Managment.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class LoginController : Controller
    {
        private readonly ILoginService _loginService;
        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var result = await _loginService.LoginAsync(request);
            return result;
        }

    }
}

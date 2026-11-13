using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service_Managment.Application.DTOs;
using Service_Managment.Application.Interface;

namespace Service_Managment.API.Controllers;

[ApiController]
[Route("api/Auth")]
public class LoginController : ControllerBase
{
    private readonly ILoginService _loginService;
    private readonly IUserService _userService;
    private readonly IWorkerService _workerService;

    public LoginController(ILoginService loginService, IUserService userService, IWorkerService workerService)
    {
        _loginService = loginService;
        _userService = userService;
        _workerService = workerService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> LoginAsync(LoginRequest request)
    {
        try { return Ok(await _loginService.LoginAsync(request)); }
        catch (InvalidOperationException e) { return Unauthorized(new { message = e.Message }); }
    }

    [AllowAnonymous]
    [HttpPost("register-user")]
    public async Task<ActionResult<AuthResponse>> RegisterUser(RegisterUserRequest request)
    {
        try { return Ok(await _userService.RegisterUserAsync(request)); }
        catch (InvalidOperationException e) { return Conflict(new { message = e.Message }); }
    }

    [AllowAnonymous]
    [HttpPost("register-worker")]
    public async Task<ActionResult<AuthResponse>> RegisterWorker(RegisterWorkerRequest request)
    {
        try { return Ok(await _workerService.RegisterWorkerAsync(request)); }
        catch (InvalidOperationException e) { return Conflict(new { message = e.Message }); }
    }
}

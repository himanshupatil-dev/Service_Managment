using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service_Managment.Application.DTOs;
using Service_Managment.Application.Interface;

namespace Service_Managment.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class WorkerController : Controller
    {
        private readonly IWorkerService _workerService;
        public WorkerController(IWorkerService workerService)
        {
            _workerService = workerService;
        }

        [AllowAnonymous]
        [HttpPost("register/worker")]
        public async Task<IActionResult> Registerworker(RegisterWorkerRequest request)
        {
            var result = await _workerService.RegisterWorkerAsync(request);
            return Ok(result);

        }
    }
}

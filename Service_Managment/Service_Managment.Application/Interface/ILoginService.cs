using Service_Managment.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Managment.Application.Interface
{
    public interface ILoginService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        //Task<LoginResponse> RegisterUserAsync(RegisterUserRequest request);
    }
}

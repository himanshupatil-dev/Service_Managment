using Service_Managment.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Managment.Application.Interface
{
    public interface IWorkerService
    {
        Task<AuthResponse> RegisterWorkerAsync(RegisterWorkerRequest request);
    }
}

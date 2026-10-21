using Service_Managment.Application.DTOs;
using Service_Managment.Domain.Entities;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Service_Managment.Application.Interface
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<bool> HasActiveAppointmentAsync(int workerId);

        Task<Appointment?> GetWithDetailsAsync(int appointmentId);

        Task<Appointment?> GetAppointmentWithWorkerAsync(int appointmentId);

        Task<List<Appointment>> GetUserAppointmentsAsync(int userId);

        Task<List<Appointment>> GetWorkerAppointmentAsync(int workerId);

        //Task<List<Worker>> GetWorkerByCategoryAsync(int workerId,string Category);
    }
}
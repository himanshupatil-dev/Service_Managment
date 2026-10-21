using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Service_Managment.Domain.Entities;
using Service_Managment.Domain.Enum;
using Service_Managment.Infrastructure.Persistence;
using Service_Managment.Application.Interface;
using Service_Managment.Domain.Enum.Service_Managment.Domain.Enum;

namespace Service_Managment.Infrastructure.Repositories
{
    public class AppointmentRepository
        : GenericRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<bool> HasActiveAppointmentAsync(int workerId)
        {
            return await _dbSet.AnyAsync(a =>
                a.WorkerId == workerId &&
                a.Status == Status.InProgress);
        }

        public async Task<Appointment?> GetWithDetailsAsync(int appointmentId)
        {
            return await _dbSet
                .Include(a => a.User)
                .Include(a => a.Worker)
                .ThenInclude(w => w.User)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);
        }

        public async Task<Appointment?> GetAppointmentWithWorkerAsync(int appointmentId)
        {
            return await _dbSet
                .Include(a => a.Worker)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);
        }

        public async Task<List<Appointment>> GetUserAppointmentsAsync(int userId)
        {
            return await _dbSet
                .Where(a => a.UserId == userId)
                .Include(a => a.User)
                .Include(a => a.Worker)
                    .ThenInclude(w => w.User)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetWorkerAppointmentAsync(int workerId)
        {
            return await _dbSet
                .Where(w => w.WorkerId == workerId)
                .Include(w => w.User)
                .Include(w => w.Worker)
                    .ThenInclude(w => w.User)
                .ToListAsync();
        }

        //public async Task<List<Worker>> GetWorkerByCategoryAsync(int workerId,string Category)
        //{
        //    return await _dbSet
        //    .Where(u => u.Worker != null && u.Worker.Category == Category)
        //    .Include(u => u.Worker)
        //    .ToListAsync();
        //}

    }
}
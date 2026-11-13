using Microsoft.EntityFrameworkCore;
using Service_Managment.Application.Interface;
using Service_Managment.Domain.Entities;
using Service_Managment.Domain.Enum.Service_Managment.Domain.Enum;
using Service_Managment.Infrastructure.Persistence;

namespace Service_Managment.Infrastructure.Repositories;

public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(ApplicationDbContext context) : base(context) { }

    public Task<bool> HasActiveAppointmentAsync(int workerId) => _dbSet.AnyAsync(a =>
        a.WorkerId == workerId && (a.Status == Status.Accepted || a.Status == Status.InProgress));

    public Task<Appointment?> GetWithDetailsAsync(int appointmentId) => _dbSet
        .Include(a => a.User).Include(a => a.Worker).ThenInclude(w => w.User)
        .Include(a => a.RatingDetails).Include(a => a.Payments).FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

    public Task<Appointment?> GetAppointmentWithWorkerAsync(int appointmentId) => GetWithDetailsAsync(appointmentId);

    public Task<List<Appointment>> GetUserAppointmentsAsync(int userId) => _dbSet
        .Where(a => a.UserId == userId).Include(a => a.User).Include(a => a.Worker).ThenInclude(w => w.User)
        .Include(a => a.RatingDetails).Include(a => a.Payments).ToListAsync();

    public Task<List<Appointment>> GetWorkerAppointmentAsync(int workerId) => _dbSet
        .Where(a => a.WorkerId == workerId).Include(a => a.User).Include(a => a.Worker).ThenInclude(w => w.User)
        .Include(a => a.RatingDetails).Include(a => a.Payments).ToListAsync();
}

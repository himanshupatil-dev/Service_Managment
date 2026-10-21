using Service_Managment.Application.DTOs;
using Service_Managment.Application.Interface;
using Service_Managment.Domain.Entities;
using Service_Managment.Domain.Enum;
using Service_Managment.Domain.Enum.Service_Managment.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace Service_Managment.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IGenericRepository<Worker> _genericRepository;
        private readonly IGenericRepository<OtpVerification> _otpVerification;
        

        public AppointmentService(
            IAppointmentRepository appointmentRepository,
            IGenericRepository<Worker> genericRepository,
            IGenericRepository<OtpVerification> otpVerification)
        {
            _appointmentRepository = appointmentRepository;
            _genericRepository = genericRepository;
            _otpVerification = otpVerification;
        }

        public async Task<AppointmentResponse> BookAppointmentAsync(BookAppointmentRequest request, int userId)
        {
            var worker = await _genericRepository.GetByIdAsync(request.WorkerId);

            if (worker == null)
            {
                throw new InvalidOperationException("Worker not found");
            }

            if(userId == worker.UserId)
            {
                throw new InvalidOperationException("User cannot book an appointment with themselves");
            }

            if (!worker.IsAvailable)
            {
                throw new InvalidOperationException("Worker is not available");
            }

            if (await _appointmentRepository.HasActiveAppointmentAsync(request.WorkerId ))
            {
                throw new InvalidOperationException(
                    "Worker already has an active appointment");
            }

            var newAppointment = new Appointment
            {
                UserId = userId,
                WorkerId = request.WorkerId,
                ProblemDescription = request.ProblemDescription,
                PartsRequired = request.PartsRequired,
                Status = Domain.Enum.Service_Managment.Domain.Enum.Status.Pending,
                //Otp = request.OtpCode,
                CreatedAt = DateTime.UtcNow,
                //AppointmentId = 0, // This will be set by the database
            };

            await _appointmentRepository.AddAsync(newAppointment);
            await _appointmentRepository.SaveChangesAsync();

            var created = await _appointmentRepository.GetWithDetailsAsync(newAppointment.AppointmentId);

            if (created == null)
            {
                throw new InvalidOperationException("Appointment could not be loaded after saving");
            }

            return new AppointmentResponse
            {
                AppointmentId = created.AppointmentId,
                UserId = created.UserId,
                Status = created.Status.ToString(),
                UserName = created.User?.UserName ?? string.Empty,
                WorkerId = created.WorkerId,
                WorkerName = created.Worker?.User?.UserName ?? string.Empty,
                PartsRequired = created.PartsRequired,
                ProblemDescription = created.ProblemDescription,
                CreatedAt = created.CreatedAt,
                //FailureReason = a.FailureReason,
            };

        }

        public async Task<AppointmentResponse> AcceptAppointmentAsync(int appointmentId, int workerId)
        {
            //check if worker exists
            var workers = await _genericRepository.FindAsync(w => w.UserId == workerId);
            var worker = workers.FirstOrDefault();

            if (worker==null)
            {
                throw new InvalidOperationException("Worker does not Exist");
            }

            //check it the worker is available or not

            if (!worker.IsAvailable)
            {
                throw new InvalidOperationException("Worker is not available");
            }

            //check if the appointment exist
            var appointment = await _appointmentRepository.GetWithDetailsAsync(appointmentId);
            if(appointment == null)
            {
                throw new InvalidOperationException("Appointment not found");
            }
            //check if the appointment is pending state or not
            if (appointment.Status != Domain.Enum.Service_Managment.Domain.Enum.Status.Pending)
            {
                throw new InvalidOperationException("Appointment is not in a pending state");
            }


            //check if the appointment is assigned to the right worker
            if (appointment.WorkerId != worker.WorkerId)
            {
                throw new InvalidOperationException("This appointment is not assigned to this worker");
            }

            //Accepting the appointment
            appointment.Status = Domain.Enum.Service_Managment.Domain.Enum.Status.Accepted;
            worker.IsAvailable = false;

            await _appointmentRepository.SaveChangesAsync();

            return new AppointmentResponse
            {
                AppointmentId = appointment.AppointmentId,
                UserId = appointment.UserId,
                UserName = appointment.User?.UserName ?? string.Empty,
                ProblemDescription = appointment.ProblemDescription,
                PartsRequired = appointment.PartsRequired,
                WorkerId = worker.WorkerId,
                WorkerName = worker.User?.UserName ?? string.Empty,
                Status = appointment.Status.ToString(),

            };
        }

        public async Task<String> RejectAppointmentAsync(int appointmentId, int workerId)
        {
            var workers = await _genericRepository.FindAsync(w => w.UserId == workerId);
            var worker = workers.FirstOrDefault();


            //check appointment exists
            var appointment = await _appointmentRepository.GetWithDetailsAsync(appointmentId);
            if(appointment == null)
            {
                throw new InvalidOperationException("Appointment not found");
            }


            //Appointment must be in pending state to be rejected
            if (appointment.Status != Domain.Enum.Service_Managment.Domain.Enum.Status.Pending)
            {
                throw new InvalidOperationException("Appointment is not in a pending state");
            }


            //check if the appointment is assigned to the right worker
            if(appointment.WorkerId != worker.WorkerId)
            {
                throw new InvalidOperationException("This appointment is not assigned to this worker");
            }

            // reject the appointment
            appointment.Status = Domain.Enum.Service_Managment.Domain.Enum.Status.Rejected;

            //Make sure to make worker availble again
            if (appointment.Worker != null)
            {
                appointment.Worker.IsAvailable = true;
            }

            await _appointmentRepository.SaveChangesAsync();
            return "Your Appointment is rejected";
        }

        //public async Task<AppointmentResponse> StartJobAsync(int appointmentId, int workerId, string otpCode)
        //{
        //    throw new NotImplementedException();
        //}
        public async Task<AppointmentResponse> StartJobAsync([Range(1, int.MaxValue)] int appointmentId,int userId, [Required, RegularExpression(@"^\d{6}$")] string otpCode)
        {
            var appointment = await _appointmentRepository
                .GetByIdAsync(appointmentId);

            var workers = await _genericRepository.FindAsync(w => w.UserId == userId);
            var worker = workers.FirstOrDefault();

            if (appointment == null)
            {
                throw new InvalidOperationException(
                    "Appointment does not exist");
            }

            // Get Worker using logged-in UserId
            //var workers = await _genericRepository.FindAsync(w => w.UserId == userId);
            //var worker = workers.FirstOrDefault();

            if (worker == null)
            {
                throw new InvalidOperationException("Worker does not exist");
            }

            // Appointment must be accepted
            if (appointment.Status != Status.Accepted)
            {
                throw new InvalidOperationException("Appointment is not Accepted yet");
            }

            // Make sure this worker owns the appointment
            if (appointment.WorkerId != worker.WorkerId)
            {
                throw new InvalidOperationException("This appointment is not assigned to this worker");
            }

            // Check whether THIS worker already has another active appointment
            var hasActiveAppointment =await _appointmentRepository.HasActiveAppointmentAsync(worker.WorkerId);

            if (hasActiveAppointment)
            {
                throw new InvalidOperationException("Worker has an active appointment");
            }

            // OTP validation
            // if (appointment.Otp != otpCode)
            // {
            //     throw new InvalidOperationException("Wrong OTP");
            // }

            appointment.Status = Status.InProgress;

            await _appointmentRepository.SaveChangesAsync();

            return new AppointmentResponse
            {
                AppointmentId = appointment.AppointmentId,
                WorkerId = worker.WorkerId,
                Status = appointment.Status.ToString()
            };
        }

        public async Task<AppointmentResponse> CompleteJobAsync(int appointmentId, int workerId)
        {
            //checking if the appointment is valid or not
            var appointment = await _appointmentRepository.GetAppointmentWithWorkerAsync(appointmentId);
            if (appointment == null)
            {
                throw new InvalidOperationException("Appointment Does not exist");
            }

            //Checking if the job has started or not
            if (appointment.Status != Domain.Enum.Service_Managment.Domain.Enum.Status.InProgress)
            {
                throw new InvalidOperationException("Start the Job First");
            }

            var JobDone = new Rating
            {
                //Score = appointment.RatingScore,
                AppointmentId = appointment.AppointmentId,
                WorkerId = appointment.WorkerId,
                //UserId = appointment.UserId,
                //CreatedAt = DateTime.UtcNow
            };

            appointment.Status = Domain.Enum.Service_Managment.Domain.Enum.Status.Completed;
            appointment.Worker.IsAvailable = true;
            await _appointmentRepository.SaveChangesAsync();
         

            return new AppointmentResponse
            {
                AppointmentId = appointment.AppointmentId,
                UserId = appointment.UserId,
                WorkerId = workerId,
                Status = appointment.Status.ToString(),
                //Rating = appointment.RatingDetails?.Score,
            };
        }

        public async Task<AppointmentResponse> CannotCompleteJobAsync([Range(1, int.MaxValue)] int appointmentId, [Range(1, int.MaxValue)] int workerId, [Required, StringLength(500, MinimumLength = 10)] string reason)
        {
            var appointment = await _appointmentRepository.GetAppointmentWithWorkerAsync(appointmentId);
            if (appointment == null)
            {
                throw new InvalidOperationException("Appointment does not exist");
            }

            if (appointment.Status != Domain.Enum.Service_Managment.Domain.Enum.Status.InProgress)
            {
                throw new InvalidOperationException("Accept and Start the Job First");
            }

            var failurereason = new Appointment
            {
                AppointmentId = appointment.AppointmentId,
                WorkerId = appointment.WorkerId,
                FailureReason = appointment.FailureReason
            };

            appointment.Status = Domain.Enum.Service_Managment.Domain.Enum.Status.CannotComplete;
            appointment.Worker.IsAvailable = true;
            await _appointmentRepository.SaveChangesAsync();

            return new AppointmentResponse
            {
                AppointmentId = appointment.AppointmentId,
                WorkerId = workerId,
                Status = appointment.Status.ToString(),
                FailureReason = appointment.FailureReason.ToString()
            };
        }

        public async Task<AppointmentResponse> CancelAppointmentAsync(int appointmentId, int userId)
        {
            var appointment = await _appointmentRepository.GetAppointmentWithWorkerAsync(appointmentId);
            if (appointment == null)
            {
                throw new InvalidOperationException("Appointment Does not Exist");
            }
            
            if(appointment.Status != Status.Accepted
               && appointment.Status != Status.Pending)
            {
                throw new InvalidOperationException("Cannot Cancel the Appointment");
            }

            var cancelappointment = new Appointment
            {
                AppointmentId = appointment.AppointmentId,
                UserId = appointment.UserId,
               
            };

            appointment.Status = Status.Cancelled;
            appointment.Worker.IsAvailable = true;
            await _appointmentRepository.SaveChangesAsync();

            return new AppointmentResponse
            {
                AppointmentId = appointment.AppointmentId,
                UserId= appointment.UserId,
                Status = appointment.Status.ToString(),
            };
        }

        public async Task<List<AppointmentResponse>> GetUserAppointmentsAsync(int userId)
        {
            var appointments = await _appointmentRepository
                .GetUserAppointmentsAsync(userId);

            return appointments.Select(a => new AppointmentResponse
            {
                AppointmentId = a.AppointmentId,
                UserId = a.UserId,
                UserName = a.User?.UserName ?? string.Empty,

                WorkerId = a.WorkerId,
                WorkerName = a.Worker?.User?.UserName ?? string.Empty,

                Status = a.Status.ToString(),
                PartsRequired = a.PartsRequired,
                ProblemDescription = a.ProblemDescription,
                CreatedAt = a.CreatedAt,
                FailureReason = a.FailureReason
            }).ToList();
        }

        public async Task<List<AppointmentResponse>> GetWorkerAppointmentsAsync(int workerId)
        {
            var appointments = await _appointmentRepository
                .GetWorkerAppointmentAsync(workerId);

            return appointments.Select(a => new AppointmentResponse
            {
                WorkerId = a.WorkerId,
                WorkerName = a.Worker?.User?.UserName ?? string.Empty,
                
                AppointmentId = a.AppointmentId,
                UserId = a.UserId,
                UserName = a.User?.UserName ?? string.Empty,

                
                Status = a.Status.ToString(),
                PartsRequired = a.PartsRequired,
                ProblemDescription = a.ProblemDescription,
                CreatedAt = a.CreatedAt,
                //FailureReason = a.FailureReason
            }).ToList();
        }

        public async Task GenerateOtpAsync(int appointmentId,int userId)
        {
            // 1. Get appointment
            var appointment =
                await _appointmentRepository.GetByIdAsync(appointmentId);

            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found.");

            // 2. Get worker using logged-in user's UserId
            var workers = await _genericRepository.FindAsync(w => w.UserId == userId);

            var worker = workers.FirstOrDefault();

            if (worker == null)
                throw new InvalidOperationException("Worker not found.");

            // 3. Check worker owns appointment
            if (appointment.WorkerId != worker.WorkerId)
            {
                throw new InvalidOperationException("Appointment is not assigned to this worker.");
            }

            // 4. Check appointment status
            if (appointment.Status != Status.Accepted)
            {
                throw new InvalidOperationException(
                    "Appointment is not in Accepted state.");
            }

            // 5. Generate OTP
            var otp = GenerateOtp();

            // 6. Hash OTP and store hash
            appointment.OtpHash = HashOtp(otp);

            // 7. OTP expires 5 minutes from NOW
            appointment.OtpExpiresAt =DateTime.UtcNow.AddMinutes(5);

            // 8. Reset attempts if you're using attempts
            // appointment.OtpAttempts = 0;

            // 9. Save
            await _appointmentRepository.SaveChangesAsync();

            // 10. Publish OTP + notification data to Kafka
        }

        //public async Task<List<Worker>> GetWorkerByCategoryAsync(int workerId,string Category)
        //{

        //}


        private static AppointmentResponse MapToResponse(Appointment a)
        {
            return new AppointmentResponse
            {
                AppointmentId = a.AppointmentId,
                UserId = a.UserId,
                Status = a.Status.ToString(),
                UserName = a.User?.UserName ?? string.Empty,
                WorkerId = a.WorkerId,
                WorkerName = a.Worker?.User?.UserName ?? string.Empty,
                PartsRequired = a.PartsRequired,
                ProblemDescription = a.ProblemDescription,
                CreatedAt = a.CreatedAt,
                //FailureReason = a.FailureReason,
            };

        }
        private string GenerateOtp()
        {
            return RandomNumberGenerator
                .GetInt32(100000, 1000000)
                .ToString();
        }

        private string HashOtp(string otp)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(otp);

            byte[] hash = SHA256.HashData(bytes);

            return Convert.ToHexString(hash);
        }


    }
}
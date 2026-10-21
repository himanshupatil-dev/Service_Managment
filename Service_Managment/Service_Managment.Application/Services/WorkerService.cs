using Microsoft.AspNetCore.Identity;
using Service_Managment.Application.DTOs;
using Service_Managment.Application.Interface;
using Service_Managment.Domain.Entities;
using Service_Managment.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Managment.Application.Services
{
    public class WorkerService : IWorkerService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<Worker> _workerRepository;

        public WorkerService(
            IGenericRepository<User> userRepository,
            IGenericRepository<Worker> workerRepository)
        {
            _userRepository = userRepository;
            _workerRepository = workerRepository;
        }

        public async Task<AuthResponse> RegisterWorkerAsync(RegisterWorkerRequest request)
        {
            // 1. Check whether email already exists
            var existingUser = await _userRepository
                .FindAsync(u => u.UserEmail == request.WorkerEmail);

            if (existingUser.Any())
            {
                throw new InvalidOperationException("Email already exists.");
            }

            var existingMobile = await _userRepository.FindAsync(u => u.MobileNo == request.MobileNo);

            if (existingMobile.Any())
            {
                throw new InvalidOperationException("Mobile number already exists.");
            }

            // 2. Hash password
            var hasher = new PasswordHasher<User>();
            var passwordHash = hasher.HashPassword(null!, request.Password);

            // 3. Create User
            var user = new User
            {
                UserName = request.WorkerName,
                UserEmail = request.WorkerEmail,
                MobileNo = request.MobileNo,
                Address = request.Address,
                PasswordHash = passwordHash,
                IsMobileVerified = false,
                CreatedAt = DateTime.UtcNow,
                Role = UserRole.Worker
            };

            // 4. Save User first
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            // Now user.UserId contains the generated database ID

            // 5. Create Worker
            var worker = new Worker
            {
                UserId = user.UserId,

                Category = request.Category,
                Qualification = request.Qualification,
                License = request.License,
                Experience = request.Experience,
                PricingModel = request.PricingModel,
                Price = request.Price,

                VerificationStatus = VerificationStatus.Pending,
                IsAvailable = true,
                IsBlocked = false,
                CoinBalance = 0
            };

            // 6. Save Worker
            await _workerRepository.AddAsync(worker);
            await _workerRepository.SaveChangesAsync();

            // 7. Return response
            return new AuthResponse
            {
                WorkerId = worker.WorkerId,
                UserName = user.UserName,
                Role = "Worker"
            };
        }
    }
}

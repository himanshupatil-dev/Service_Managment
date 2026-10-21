using Service_Managment.Application.DTOs;
using Service_Managment.Application.Interface;
using Service_Managment.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service_Managment.Domain.Enum;
using System.Reflection.Metadata.Ecma335;

namespace Service_Managment.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userRepository;
        
        public UserService(IGenericRepository<User> userRepository)
        {
            _userRepository = userRepository;
            
        }
        public async Task<AuthResponse> RegisterUserAsync(RegisterUserRequest request)
        {
            var checkEmail = await _userRepository.FindAsync(a => a.UserEmail == request.UserEmail);

            if (checkEmail.Any())
            {
                throw new InvalidOperationException("Email already exists");
            }

            var hasher = new PasswordHasher<User>();
            string hashedPassword = hasher.HashPassword(null!, request.Password);

            var registeruser = new User
            {
                UserName = request.UserName,
                UserEmail = request.UserEmail,
                MobileNo = request.MobileNo,
                IsMobileVerified = false,
                Address = request.Address,
                PasswordHash = hashedPassword,
                Longitude = request.Longitude,
                Latitude = request.Latitude,
                CreatedAt = DateTime.UtcNow,
                Role = UserRole.User
            };

            await _userRepository.AddAsync(registeruser);
            await _userRepository.SaveChangesAsync();

            var returnuser = new AuthResponse
            {
                WorkerId = registeruser.UserId,
                UserName = registeruser.UserName,
                Role = registeruser.Role.ToString()
            };

            return returnuser;
        }

        
    }
}

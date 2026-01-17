using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using YapperzAPI.Data;
using YapperzAPI.Dtos.Users;
using YapperzAPI.Extensions;
using YapperzAPI.Models;
using YapperzAPI.Services.Interfaces;

namespace YapperzAPI.Services
{
    public class UsersServices : IUsersService
    {
        public AppDbContext _appDbContext;
        public IPasswordHasher< User > _passwordHasher;

        public UsersServices(AppDbContext appDbContext, IPasswordHasher<User> passwordHasher) 
        {
            _appDbContext = appDbContext;
            _passwordHasher = passwordHasher;
        }

        public async Task<UsersDto> SignUpAsync(UserSignUpDto request)
        {
            if (await _appDbContext.Users.AnyAsync(u => u.Username == request.Username))
            {
                throw new InvalidOperationException("Username already exists.");
            }

            if (await _appDbContext.Users.AnyAsync(u => u.Email == request.Email))
            {
                throw new InvalidOperationException("Email already exists.");
            }

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                DisplayName = request.DisplayName,
                AvatarPath = request.AvatarPath
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            _appDbContext.Users.Add(user);
            await _appDbContext.SaveChangesAsync();

            return user.ToDto();
        }

        public async Task<UsersDto?> AuthenticateUser(UserLoginDto request)
        {
            var lowered = request.UsernameOrEmail.ToLowerInvariant();
            var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == lowered || u.Email.ToLower() == lowered);
            if (user is null)
            {
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded
                ? user.ToDto()
                : null;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _appDbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<UsersDto?> GetProfileAsync(int id)
        {
            var user = await GetByIdAsync(id);
            return user?.ToDto();
        }
    }
}

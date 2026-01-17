using YapperzAPI.Models;
using YapperzAPI.Dtos.Users;

namespace YapperzAPI.Services.Interfaces
{
    public interface IUsersService
    {
        Task<UsersDto> SignUpAsync(UserSignUpDto request);
        Task<UsersDto?> AuthenticateUser(UserLoginDto request);
        Task<UsersDto?> GetProfileAsync(int id);
    }
}

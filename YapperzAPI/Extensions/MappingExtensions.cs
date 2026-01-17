using YapperzAPI.Dtos.Users;
using YapperzAPI.Models;

namespace YapperzAPI.Extensions
{
    public static class MappingExtensions
    {
        public static UsersDto ToDto(this User user)
        {
            return new UsersDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                DisplayName = user.DisplayName,
                AvatarPath = user.AvatarPath,
                Bio = user.Bio,
                Room = user.Room,
                JoinedAt = user.JoinedAt
            };
        }
    }
}

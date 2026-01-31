using Microsoft.AspNetCore.SignalR;
using YapperzAPI.Dtos.Users;
using YapperzAPI.Services.Interfaces;

namespace YapperzAPI.Hubs
{
    public class ChatroomHub: Hub
    {
        private readonly IChatroomService _chatroomService;

        public ChatroomHub(IChatroomService chatroomService)
        {
            _chatroomService = chatroomService;
        }

        public async Task JoinRoomGroup(string roomCode)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
        }

        // You can also add a method to leave a group
        public async Task LeaveRoomGroup(string roomCode)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomCode);
        }

        public async Task SendMessage(string roomCode, int userId, string message)
        {
            await Clients.Group(roomCode).SendAsync("ReceiveMessage", userId, message);
        }

        public async Task NotifyNewPlayerJoined(string roomCode, int userId)
        {
            var users = await _chatroomService.GetUsersByRoomCodeAsync(roomCode);
            var user = users.FirstOrDefault(u => u.Id == userId);

            if (user is null)
            {
                return;
            }

            await Clients.Group(roomCode).SendAsync("NewPlayerJoined", user);
        }
    }
}

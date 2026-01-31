using Microsoft.AspNetCore.SignalR;

namespace YapperzAPI.Hubs
{
    public class ChatroomHub: Hub
    {
        public async Task SendMessage(int userId, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", userId, message);
        }
    }
}

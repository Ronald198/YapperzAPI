using YapperzAPI.Dtos.Chatroom;

namespace YapperzAPI.Services.Interfaces
{
    public interface IChatroomService
    {
        Task<bool> JoinChatroomAsync(ChatroomJoinLeaveDto request);
        Task<bool> LeaveChatroomAsync(ChatroomJoinLeaveDto request);
        Task<ChatroomDto?> GetRoomByCodeAsync(string roomCode);
    }
}

using Microsoft.EntityFrameworkCore;
using System.Globalization;
using YapperzAPI.Data;
using YapperzAPI.Dtos.Chatroom;
using YapperzAPI.Extensions;
using YapperzAPI.Models;
using YapperzAPI.Services.Interfaces;

namespace YapperzAPI.Services
{
    public class ChatroomService : IChatroomService
    {
        public AppDbContext _appDbContext;

        public ChatroomService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<ChatRoom?> GetByCodeAsync(string roomCode)
        {
            return await _appDbContext.Chatrooms.FirstOrDefaultAsync(c => c.Code == roomCode);
            /*
            return await _appDbContext.Include(c => c.Users).FirstOrDefaultAsync(c => c.Code == roomCode);
            //Eager-load Users so the DTO mapping can include them
            */
        }

        public async Task<ChatroomDto?> GetRoomByCodeAsync(string roomCode)
        {
            var room = await GetByCodeAsync(roomCode);
            return room?.ToDto();
        }

        public async Task<bool> JoinChatroomAsync(ChatroomJoinLeaveDto request)
        {
            ChatRoom? chatRoom = _appDbContext.Chatrooms.SingleOrDefault(c => c.Code == request.RoomCode);

            if (chatRoom == null)
            {
                throw new InvalidOperationException("Room does not exist. Check code.");
            }
            else
            {
                if (chatRoom.Users.Count >= chatRoom.MaxPlayers)
                {
                    throw new InvalidOperationException("Room is full.");
                }
                if (chatRoom.Status == RoomStatus.Closed)
                {
                    throw new InvalidOperationException("Room is closed.");
                }

                User? user = _appDbContext.Users.FirstOrDefault(u => u.Id == request.UserId);

                if (user == null)
                {
                    throw new InvalidOperationException("User does not exist.");
                }
                else
                {
                    chatRoom.Users.Add(user);
                    user.Room = chatRoom;
                    await _appDbContext.SaveChangesAsync();
                    return true;
                }
            }
        }

        public async Task<bool> LeaveChatroomAsync(ChatroomJoinLeaveDto request)
        {
            User? user = _appDbContext.Users.FirstOrDefault(u => u.Id == request.UserId);
            ChatRoom? chatRoom = _appDbContext.Chatrooms.SingleOrDefault(c => c.Code == request.RoomCode);

            if (user == null)
            {
                throw new InvalidOperationException("User does not exist.");
            }
            else
            {
                user.Room = null;
                await _appDbContext.SaveChangesAsync();

                if (chatRoom == null)
                {
                    throw new InvalidOperationException("Room no longer exists.");
                }
                else
                {
                    chatRoom.Users.Remove(user);
                    await _appDbContext.SaveChangesAsync();
                    return true;
                }
            }
        }
    }
}

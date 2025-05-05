using WebChat.Application.DTOs;
using WebChat.Application.Models;

namespace WebChat.Application.Services
{
    public interface IRoomService
    {
        Task CreateRoomAsync(RegisterRoomDTO dto);
        Task<IEnumerable<Room>> GetRoomsAsync();
    }
}
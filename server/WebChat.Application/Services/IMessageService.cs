using WebChat.Application.Models;

namespace WebChat.Application.Services
{
    public interface IMessageService
    {
        Task SaveMessageAsync(Message message);
        Task<IEnumerable<Message>> GetChatHistoryAsync(Guid roomId, int limit = 50);
    }
}
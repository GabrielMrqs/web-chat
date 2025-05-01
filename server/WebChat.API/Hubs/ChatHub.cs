using Microsoft.AspNetCore.SignalR;
using WebChat.Application.Models;
using WebChat.Application.Services;

namespace WebChat.API.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;

        public ChatHub(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task SendMessage(Message message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);

            await _messageService.SaveMessageAsync(message);
        }
    }
}

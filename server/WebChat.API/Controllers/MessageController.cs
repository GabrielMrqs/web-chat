using Microsoft.AspNetCore.Mvc;
using WebChat.Application.DTOs;
using WebChat.Application.Models;
using WebChat.Application.Services;
using WebChat.Infra.Services;

namespace WebChat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("{roomId}")]
        public async Task<ActionResult<IEnumerable<Message>>> GetChatHistoryAsync(Guid roomId, [FromQuery] int limit = 50)
        {
            try
            {
                var messages = await _messageService.GetChatHistoryAsync(roomId, limit);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving chat history: {ex.Message}");
            }
        }
    }
}
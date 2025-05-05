using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebChat.Application.DTOs;
using WebChat.Application.Models;
using WebChat.Application.Services;

namespace WebChat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> Get()
        {
            try
            {
                var rooms = await _roomService.GetRoomsAsync();
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving chat history: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] RegisterRoomDTO dto)
        {
            try
            {
                await _roomService.CreateRoomAsync(dto);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving chat history: {ex.Message}");
            }
        }
    }
}
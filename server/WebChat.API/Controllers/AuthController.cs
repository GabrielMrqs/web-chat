using Microsoft.AspNetCore.Mvc;
using WebChat.Application.DTOs;
using WebChat.Application.Services;

namespace WebChat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(IAuthService authService, IJwtTokenService jwtTokenService)
        {
            _authService = authService;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register([FromBody] RegisterUserDTO dto)
        {
            try
            {
                await _authService.RegisterUserAsync(dto);


                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving chat history: {ex.Message}");
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login([FromBody] LoginDTO dto)
        {
            try
            {
                await _authService.LoginAsync(dto);

                var token = _jwtTokenService.GenerateToken(dto.Email);

                return Ok(token);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving chat history: {ex.Message}");
            }
        }
    }
}
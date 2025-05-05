using WebChat.Application.DTOs;

namespace WebChat.Application.Services
{
    public interface IAuthService
    {
        Task LoginAsync(LoginDTO dto);
        Task RegisterUserAsync(RegisterUserDTO dto);
    }
}

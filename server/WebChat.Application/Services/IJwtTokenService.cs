using WebChat.Application.DTOs;

namespace WebChat.Application.Services
{
    public interface IJwtTokenService
    {
        WebToken GenerateToken(string nameIdentifier);
    }
}

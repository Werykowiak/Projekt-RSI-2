using Projekt_RSI_2_BackEnd.DTOs;

namespace Projekt_RSI_2_BackEnd.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string Message)> RegisterAsync(RegisterDto request);
        Task<(bool Success, string Token, string ErrorMessage)> LoginAsync(LoginDto request);
    }
}

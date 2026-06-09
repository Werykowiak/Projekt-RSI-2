using Projekt_RSI_2_BackEnd.DTOs;
using Projekt_RSI_2_BackEnd.Models;

namespace Projekt_RSI_2_BackEnd.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string Message)> RegisterAsync(RegisterDto request);
        Task<(bool Success, string Token, User? User, string ErrorMessage)> LoginAsync(LoginDto request);
    }
}

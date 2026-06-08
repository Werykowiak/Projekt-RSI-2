namespace Projekt_RSI_2_BackEnd.DTOs
{
    public record RegisterDto(string Email, string Password, string FirstName, string LastName);

    public record LoginDto(string Email, string Password);
}

using Projekt_RSI_2_BackEnd.Models;

namespace Projekt_RSI_2_BackEnd.Interfaces
{
    public interface IReservationService
    {
        Task<(bool Success, string Message, int? ReservationId)> BookTicketAsync(int trainRouteId, int numberOfSeats, int userId);

        Task<(bool Success, Reservation? Data, string ErrorMessage)> GetReservationAsync(int reservationId, int userId);

        Task<(bool Success, byte[] PdfBytes, string FileName)> GetReservationPdfAsync(int reservationId, int userId);
    }
}

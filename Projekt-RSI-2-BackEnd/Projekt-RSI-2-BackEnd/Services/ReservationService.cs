using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Projekt_RSI_2_BackEnd.Data;
using Projekt_RSI_2_BackEnd.Hubs;
using Projekt_RSI_2_BackEnd.Interfaces;
using Projekt_RSI_2_BackEnd.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Text;

namespace Projekt_RSI_2_BackEnd.Services
{
    public class ReservationService : IReservationService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<BookingHub> _hubContext;

        public ReservationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message, int? ReservationId)> BookTicketAsync(int trainRouteId, int numberOfSeats, int userId)
        {
            var route = await _context.TrainRoutes.FindAsync(trainRouteId);
            if (route == null) return (false, "Trasa nie istnieje.", null);

            if (route.AvailableSeats < numberOfSeats)
                return (false, "Brak wystarczającej liczby miejsc.", null);

            var reservation = new Reservation
            {
                TrainRouteId = route.Id,
                UserId = userId,
                ReservationDate = DateTime.Now,
                NumberOfSeats = numberOfSeats
            };

            route.AvailableSeats -= numberOfSeats;
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("UpdateSeats", route.Id, route.AvailableSeats);

            return (true, "Kupiono bilet.", reservation.Id);
        }

        public async Task<(bool Success, Reservation? Data, string ErrorMessage)> GetReservationAsync(int reservationId, int userId)
        {
            var res = await _context.Reservations
                .Include(r => r.TrainRoute)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (res == null) return (false, null, "Nie znaleziono rezerwacji.");

            if (res.UserId != userId) return (false, null, "Brak uprawnień do tej rezerwacji.");

            return (true, res, string.Empty);
        }

        public async Task<(bool Success, byte[] PdfBytes, string FileName)> GetReservationPdfAsync(int reservationId, int userId)
        {
            var check = await GetReservationAsync(reservationId, userId);
            if (!check.Success || check.Data == null)
                return (false, Array.Empty<byte>(), check.ErrorMessage);

            var reservation = check.Data;
            var route = reservation.TrainRoute;

            var user = await _context.Users.FindAsync(userId);
            string passengerName = user != null ? $"{user.FirstName} {user.LastName}" : "Nieznany Pasażer";
            string passengerEmail = user != null ? user.Email : "Brak adresu email";

            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().Text("Bilet na pociąg")
                        .SemiBold().FontSize(24).FontColor(Colors.Blue.Darken2);

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(x =>
                    {
                        x.Spacing(10);

                        x.Item().Text($"Imię i nazwisko: {passengerName}");
                        x.Item().Text($"Email: {passengerEmail}");
                        x.Item().Text($"ID Rezerwacji: {reservation.Id}");
                        x.Item().Text($"Data Rezerwacji: {reservation.ReservationDate:dd.MM.yyyy HH:mm}");
                        x.Item().Text($"Liczba miejsc: {reservation.NumberOfSeats}");

                        if (route != null)
                        {
                            x.Item().PaddingTop(10).Text($"Trasa: {route.DepartureCity} -> {route.ArrivalCity}").Bold();
                            x.Item().Text($"Data odjazdu: {route.DepartureTime:dd.MM.yyyy HH:mm}");
                            x.Item().Text($"Cena łączna: {route.Price * reservation.NumberOfSeats} PLN").Bold().FontColor(Colors.Green.Darken2);
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("DAWIDZIOR & WERYK TRAINS 😎");
                    });
                });
            });

            byte[] pdfBytes = document.GeneratePdf();

            return (true, pdfBytes, $"Bilet_{reservationId}.pdf");
        }
    }
}

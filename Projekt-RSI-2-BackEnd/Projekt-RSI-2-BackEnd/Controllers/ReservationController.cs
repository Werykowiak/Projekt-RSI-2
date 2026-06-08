using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projekt_RSI_2_BackEnd.DTOs;
using Projekt_RSI_2_BackEnd.Interfaces;
using Projekt_RSI_2_BackEnd.Models;
using System.Security.Claims;

namespace Projekt_RSI_2_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        [HttpPost("book")]
        public async Task<IActionResult> Book([FromBody] BookReservationDto payload)
        {
            var result = await _reservationService.BookTicketAsync(payload.TrainRouteId, payload.NumberOfSeats, GetUserId());
            if (!result.Success) return BadRequest(result.Message);

            return Ok(new { Message = result.Message, ReservationId = result.ReservationId });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _reservationService.GetReservationAsync(id, GetUserId());
            if (!result.Success)
            {
                if (result.ErrorMessage.Contains("Brak uprawnień")) return Forbid(); 
                return NotFound(result.ErrorMessage); 
            }
            return Ok(result.Data);
        }

        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> GetPdf(int id)
        {
            var result = await _reservationService.GetReservationPdfAsync(id, GetUserId());
            if (!result.Success) return BadRequest(result.FileName);

            return File(result.PdfBytes, "application/pdf", result.FileName);
        }
    }
}

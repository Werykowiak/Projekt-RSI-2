using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Projekt_RSI_2_BackEnd.Interfaces;
using Projekt_RSI_2_BackEnd.Models;

namespace Projekt_RSI_2_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainRoutesController : ControllerBase
    {
        private readonly ITrainRouteService _routeService;

        public TrainRoutesController(ITrainRouteService routeService)
        {
            _routeService = routeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _routeService.GetAllRoutesAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var route = await _routeService.GetRouteByIdAsync(id);
            return route != null ? Ok(route) : NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] TrainRoute route)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var newRoute = await _routeService.CreateRouteAsync(route);
            // Zmiana na CreatedAtAction z jawnym określeniem nazwy akcji dla pewności
            return CreatedAtAction("GetById", new { id = newRoute.Id }, newRoute); 
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] TrainRoute route)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return await _routeService.UpdateRouteAsync(route) ? NoContent() : NotFound(); 
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            return await _routeService.DeleteRouteAsync(id) ? NoContent() : NotFound();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? departureCity, [FromQuery] string? arrivalCity, [FromQuery] DateTime? date)
        {
            return Ok(await _routeService.SearchRoutesAsync(departureCity, arrivalCity, date));
        }
    }
}
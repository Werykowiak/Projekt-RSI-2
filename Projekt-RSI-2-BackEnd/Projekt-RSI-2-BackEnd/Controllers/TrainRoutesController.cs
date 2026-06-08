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
        public async Task<IActionResult> Create([FromBody] TrainRoute route)
        {
            var newRoute = await _routeService.CreateRouteAsync(route);
            return CreatedAtAction(nameof(GetById), new { id = newRoute.Id }, newRoute); 
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] TrainRoute route)
        {
            return await _routeService.UpdateRouteAsync(route) ? NoContent() : NotFound(); 
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await _routeService.DeleteRouteAsync(id) ? NoContent() : NotFound();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string from, [FromQuery] string to, [FromQuery] DateTime date)
        {
            return Ok(await _routeService.SearchRoutesAsync(from, to, date));
        }
    }
}
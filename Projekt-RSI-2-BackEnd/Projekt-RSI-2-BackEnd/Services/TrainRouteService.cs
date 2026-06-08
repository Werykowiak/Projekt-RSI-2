using Projekt_RSI_2_BackEnd.Data;
using Projekt_RSI_2_BackEnd.Interfaces;
using Projekt_RSI_2_BackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace Projekt_RSI_2_BackEnd.Services
{
    public class TrainRouteService : ITrainRouteService
    {
        private readonly AppDbContext _context;

        public TrainRouteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TrainRoute>> GetAllRoutesAsync() => await _context.TrainRoutes.ToListAsync();

        public async Task<TrainRoute?> GetRouteByIdAsync(int id) => await _context.TrainRoutes.FindAsync(id);

        public async Task<TrainRoute> CreateRouteAsync(TrainRoute trainRoute)
        {
            _context.TrainRoutes.Add(trainRoute);
            await _context.SaveChangesAsync();
            return trainRoute;
        }

        public async Task<bool> UpdateRouteAsync(TrainRoute trainRoute)
        {
            var route = await _context.TrainRoutes.FindAsync(trainRoute.Id);
            if (route == null) return false;

            route.DepartureCity = trainRoute.DepartureCity;
            route.ArrivalCity = trainRoute.ArrivalCity;
            route.DepartureTime = trainRoute.DepartureTime;
            route.ArrivalTime = trainRoute.ArrivalTime;
            route.Price = trainRoute.Price;
            route.AvailableSeats = trainRoute.AvailableSeats;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRouteAsync(int id)
        {
            var trainRoute = await _context.TrainRoutes.FindAsync(id);
            if (trainRoute == null) return false;

            _context.TrainRoutes.Remove(trainRoute);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TrainRoute>> SearchRoutesAsync(string? departureCity, string? arrivalCity, DateTime? date)
        {
            var query = _context.TrainRoutes.AsQueryable();

            if (!string.IsNullOrEmpty(departureCity))
            {
                query = query.Where(r => r.DepartureCity.ToLower().Contains(departureCity.ToLower()));
            }

            if (!string.IsNullOrEmpty(arrivalCity))
            {
                query = query.Where(r => r.ArrivalCity.ToLower().Contains(arrivalCity.ToLower()));
            }

            if (date.HasValue)
            {
                query = query.Where(r => r.DepartureTime.Date == date.Value.Date);
            }

            return await query.ToListAsync();
        }
    }
}

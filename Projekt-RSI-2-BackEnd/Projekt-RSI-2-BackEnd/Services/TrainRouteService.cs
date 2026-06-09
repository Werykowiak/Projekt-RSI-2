using Projekt_RSI_2_BackEnd.Data;
using Projekt_RSI_2_BackEnd.Interfaces;
using Projekt_RSI_2_BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Projekt_RSI_2_BackEnd.Services
{
    public class TrainRouteService : ITrainRouteService
    {
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache;
        private const string CacheVersionKey = "SearchCacheVersion";

        public TrainRouteService(AppDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        private async Task<string> GetCacheVersionAsync()
        {
            try
            {
                var version = await _cache.GetStringAsync(CacheVersionKey);
                return version ?? "0";
            }
            catch
            {
                return "0";
            }
        }

        public async Task ClearCacheAsync()
        {
            try
            {
                var version = await GetCacheVersionAsync();
                if (int.TryParse(version, out int v))
                {
                    await _cache.SetStringAsync(CacheVersionKey, (v + 1).ToString());
                }
                else
                {
                    await _cache.SetStringAsync(CacheVersionKey, "1");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Redis error in ClearCacheAsync: {ex.Message}");
            }
        }

        public async Task<IEnumerable<TrainRoute>> GetAllRoutesAsync() => await _context.TrainRoutes.ToListAsync();

        public async Task<TrainRoute?> GetRouteByIdAsync(int id) => await _context.TrainRoutes.FindAsync(id);

        public async Task<TrainRoute> CreateRouteAsync(TrainRoute trainRoute)
        {
            _context.TrainRoutes.Add(trainRoute);
            await _context.SaveChangesAsync();
            await ClearCacheAsync();
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
            await ClearCacheAsync();
            return true;
        }

        public async Task<bool> DeleteRouteAsync(int id)
        {
            var trainRoute = await _context.TrainRoutes.FindAsync(id);
            if (trainRoute == null) return false;

            _context.TrainRoutes.Remove(trainRoute);
            await _context.SaveChangesAsync();
            await ClearCacheAsync();
            return true;
        }

        public async Task<IEnumerable<TrainRoute>> SearchRoutesAsync(string? departureCity, string? arrivalCity, DateTime? date)
        {
            string version = await GetCacheVersionAsync();
            
            // tworzenie klucza dla kombinacji filtrow z uwzglednieniem wersji
            string formattedDate = date.HasValue ? date.Value.ToString("yyyy-MM-dd") : "anydate";
            string cacheKey = $"v{version}_search_{departureCity?.ToLower() ?? "any"}_{arrivalCity?.ToLower() ?? "any"}_{formattedDate}";

            try
            {
                var cachedData = await _cache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cachedData))
                {
                    return JsonSerializer.Deserialize<List<TrainRoute>>(cachedData)!;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Redis error in SearchRoutesAsync (read): {ex.Message}");
            }

            // brak danych to odpytujemy baze
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

            var routes = await query.ToListAsync();

            try
            {
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
                };

                string jsonToCache = JsonSerializer.Serialize(routes);
                await _cache.SetStringAsync(cacheKey, jsonToCache, cacheOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Redis error in SearchRoutesAsync (write): {ex.Message}");
            }

            return routes;
        }
    }
}

using Projekt_RSI_2_BackEnd.Models;

namespace Projekt_RSI_2_BackEnd.Interfaces
{
    public interface ITrainRouteService
    {
        Task<IEnumerable<TrainRoute>> GetAllRoutesAsync();
        Task<TrainRoute?> GetRouteByIdAsync(int id);
        Task<TrainRoute> CreateRouteAsync(TrainRoute trainRoute);
        Task<bool> UpdateRouteAsync(TrainRoute trainRoute);
        Task<bool> DeleteRouteAsync(int id);

        Task<IEnumerable<TrainRoute>> SearchRoutesAsync(string? from, string? to, DateTime? date);
    }
}
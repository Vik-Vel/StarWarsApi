using StarWarsApi.Models;

namespace StarWarsApi.Service
{
    public interface IStarWarsService
    {
        Task<IEnumerable<Planet>> GetAllPlanetsAsync();
        Task<Planet> GetPlanetByIdAsync(int id);
    }
}

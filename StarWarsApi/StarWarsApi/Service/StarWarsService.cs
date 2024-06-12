using Microsoft.Extensions.Caching.Memory;
using StarWarsApi.Models;
using System.Net.Http;
using System.Text.Json;

namespace StarWarsApi.Service
{
    public class StarWarsService : IStarWarsService
    {
        
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly string AllPlanetsCacheKey = "allPlanets";
        private readonly string PlanetCacheKeyPrefix = "planet_";

        public StarWarsService(HttpClient httpClient, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
        }
        //
        public async Task<IEnumerable<Planet>> GetAllPlanetsAsync()
        {
            if (!_cache.TryGetValue(AllPlanetsCacheKey, out List<Planet> planets))
            {
                //Извличане на данни от външния API:
                var response = await _httpClient.GetStringAsync("https://swapi.dev/api/planets/");
                var jsonDocument = JsonDocument.Parse(response);
                var results = jsonDocument.RootElement.GetProperty("results");

                planets = new List<Planet>();

                foreach (var result in results.EnumerateArray())
                {
                    //Извличаме различните свойства на планетата от JSON обекта и ги задаваме на съответните полета в новия обект Planet.
                    planets.Add(new Planet
                    {
                        Name = result.GetProperty("name").GetString(),
                        RotationPeriod = result.GetProperty("rotation_period").GetString(),
                        OrbitalPeriod = result.GetProperty("orbital_period").GetString(),
                        Diameter = result.GetProperty("diameter").GetString(),
                        Climate = result.GetProperty("climate").GetString(),
                        Gravity = result.GetProperty("gravity").GetString(),
                        Terrain = result.GetProperty("terrain").GetString(),
                        SurfaceWater = result.GetProperty("surface_water").GetString(),
                        Population = result.GetProperty("population").GetString(),
                        Created = result.GetProperty("created").GetString(),
                        Edited = result.GetProperty("edited").GetString(),
                    });
                }
                //Запазваме списъка с планети в кеша, използвайки ключа AllPlanetsCacheKey, за да не се налага да извличаме данните отново при следващо извикване на метода.
                _cache.Set(AllPlanetsCacheKey, planets);
            }

            return planets;
        }


        //Този метод първо проверява дали информацията за конкретната планета вече е в кеша. Ако не е, извлича данните от външен API, създава обект Planet с тези данни, съхранява го в кеша и го връща като резултат. Това подобрява ефективността, като намалява броя на мрежовите заявки и ускорява достъпа до данните.
        public async Task<Planet> GetPlanetByIdAsync(int id)
        {
            // Проверяваме дали планетата вече е кеширана с помощта на _cache.TryGetValue.Ако не е, продължаваме да извличаме данните от външния API.
            var cacheKey = $"{PlanetCacheKeyPrefix}{id}";
            if (!_cache.TryGetValue(cacheKey, out Planet planet))
            {
              var response = await _httpClient.GetStringAsync($"https://swapi.dev/api/planets/{id}/");
                var jsonDocument = JsonDocument.Parse(response);
                var result = jsonDocument.RootElement;

                planet = new Planet
                {
                    Name = result.GetProperty("name").GetString(),
                    RotationPeriod = result.GetProperty("rotation_period").GetString(),
                    OrbitalPeriod = result.GetProperty("orbital_period").GetString(),
                    Diameter = result.GetProperty("diameter").GetString(),
                    Climate = result.GetProperty("climate").GetString(),
                    Gravity = result.GetProperty("gravity").GetString(),
                    Terrain = result.GetProperty("terrain").GetString(),
                    SurfaceWater = result.GetProperty("surface_water").GetString(),
                    Population = result.GetProperty("population").GetString(),
                    Created = result.GetProperty("created").GetString(),
                    Edited = result.GetProperty("edited").GetString(),
                };

                //Запазваме обекта planet в кеша с ключа, който създадохме по-рано, за да не се налага да извличаме данните отново при следващо извикване на метода.
                _cache.Set(cacheKey, planet);
            }

            return planet;
        }
    }
}

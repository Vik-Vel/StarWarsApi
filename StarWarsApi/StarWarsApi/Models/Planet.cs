namespace StarWarsApi.Models
{
    public class Planet
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string RotationPeriod { get; set; } = null!;
        public string OrbitalPeriod { get; set; } = null!;
        public string Diameter { get; set; } = null!;
        public string Climate { get; set; } = null!;
        public string Gravity { get; set; } = null!;
        public string Terrain { get; set; } = null!;
        public string SurfaceWater { get; set; } = null!;
        public string Population { get; set; } = null!;
        public string Created { get; set; } = null!;
        public string Edited { get; set; } = null!;
    }
}

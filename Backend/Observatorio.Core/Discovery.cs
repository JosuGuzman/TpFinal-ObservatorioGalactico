namespace Observatorio.Core.Entities
{
    public class Discovery
    {
        public int DiscoveryId { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Coordinates { get; set; } = "";
        public DateTime? DiscoveryDate { get; set; }
        public int ReportedBy { get; set; }
        public int? CelestialBodyId { get; set; }
        public string Status { get; set; } = "";
        public string NASA_API_Data { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public int? VerifiedBy { get; set; }
        public  required User Reporter { get; set; }
        public required CelestialBody CelestialBody { get; set; }
        public int Rating { get; set; }
    }
}
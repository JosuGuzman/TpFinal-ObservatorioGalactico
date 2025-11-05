namespace Observatorio.Core.Models
{
    public class DiscoveryCreateRequest
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Coordinates { get; set; } = "";
        public int ReportedBy { get; set; }
        public int? CelestialBodyId { get; set; }
    }
}
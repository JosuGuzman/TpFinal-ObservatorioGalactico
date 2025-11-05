namespace Observatorio.Core.Models
{
    public class SearchRequest
    {
        public string SearchTerm { get; set; } = "";
        public string Type { get; set; } = "";
        public decimal? MaxDistance { get; set; }
        public decimal? MinMagnitude { get; set; }
        public string Status { get; set; } = "";
        public int? MinRating { get; set; }
        public string Category { get; set; } = "";
        public bool PublishedOnly { get; set; }
    }
}

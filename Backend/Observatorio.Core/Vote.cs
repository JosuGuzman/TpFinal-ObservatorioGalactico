namespace Observatorio.Core.Entities
{
    public class Vote
    {
        public int VoteId { get; set; }
        public int DiscoveryId { get; set; }
        public int UserId { get; set; }
        public string VoteType { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}
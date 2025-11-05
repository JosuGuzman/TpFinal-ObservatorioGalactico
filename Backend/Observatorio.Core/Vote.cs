namespace Observatorio.Core.Entities
{
    public class Vote
    {
        public int VoteId { get; set; }
        public int DiscoveryId { get; set; }
        public int UserId { get; set; }
        public enum VoteType { Upvote, Downvote }
        public DateTime CreatedAt { get; set; }

        public User Voter { get; set; }
        public Discovery Discovery { get; set; }
    }
}
namespace Rewards.Domain
{
    public class Reward : IEntity
    {
        public int UserId { get; set; }
        public DateTime AvailableAt { get; set; }
        public DateTime? RedeemedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}

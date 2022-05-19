namespace Rewards.Domain
{
    public class Reward : IEntity
    {
        public User User { get; set; }
        public DateTime AvailableAt { get; set; }
        public DateTime RedeemedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}

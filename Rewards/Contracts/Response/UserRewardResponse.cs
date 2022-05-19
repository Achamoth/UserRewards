namespace Rewards.Contracts.Response
{
    public class UserRewardResponse
    {
        public List<Reward> Data { get; set; }

        public class Reward
        {
            public DateTime AvailableAt { get; set; }
            public DateTime RedeemedAt { get; set; }
            public DateTime ExpiresAt { get; set; }
        }
    }
}

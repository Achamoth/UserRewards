namespace Rewards.Contracts.Response
{
    public class RedeemRewardResponse
    {
        public Reward Data { get; set; }

        public class Reward
        {
            public DateTime AvailableAt { get; set; }
            public DateTime RedeemedAt { get; set; }
            public DateTime ExpiresAt { get; set; }
        }
    }
}

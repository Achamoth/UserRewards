using Rewards.Contracts.Response;
using Rewards.Data;
using Rewards.Domain;

namespace Rewards.Command
{
    public class RedeemReward : ICommand<RedeemRewardResponse>
    {
        private readonly IStorageProvider _storageProvider;

        private int _userId;
        private DateTime _availableAt;

        public RedeemReward(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

        public void SetParameters(int userId, DateTime availableAt)
        {
            _userId = userId;
            _availableAt = availableAt;
        }

        public async Task<RedeemRewardResponse> ExecuteAsync()
        {
            var user = await _storageProvider.FindUserByIdAsync(_userId);

            if (user == null)
                throw new Exception($"User not found for id {_userId}");

            var availableAtMidnight = _availableAt.Date;
            var reward = user.Rewards.SingleOrDefault(r => r.AvailableAt == availableAtMidnight);

            if (reward == null)
                throw new Exception("No reward found");

            if (reward.ExpiresAt < DateTime.Now) // Not checking AvailableAt as documentation doesn't mention it
                throw new Exception("This reward is already expired");

            if (reward.RedeemedAt == null)
            {
                reward.RedeemedAt = DateTime.Now;
                await _storageProvider.AddOrUpdateRewardAsync(reward);
            }

            return MapToResponse(reward);
        }

        private static RedeemRewardResponse MapToResponse(Reward reward)
        {
            return new RedeemRewardResponse
            {
                Data = new RedeemRewardResponse.Reward
                {
                    AvailableAt = reward.AvailableAt,
                    RedeemedAt = reward.RedeemedAt.Value,
                    ExpiresAt = reward.ExpiresAt
                }
            };
        }
    }
}

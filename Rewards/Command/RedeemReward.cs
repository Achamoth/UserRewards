using Rewards.Contracts.Response;
using Rewards.Data;
using Rewards.Domain;
using Rewards.Middleware;

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
                throw new HttpResponseException(StatusCodes.Status404NotFound ,$"User {_userId} not found");

            var availableAtMidnight = _availableAt.Date;
            var reward = user.Rewards.SingleOrDefault(r => r.AvailableAt == availableAtMidnight);

            if (reward == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, $"This reward could not be found for user {_userId}");

            if (reward.ExpiresAt < DateTime.Now) // Not checking AvailableAt as documentation doesn't mention it
                throw new HttpResponseException(StatusCodes.Status400BadRequest, "This reward is already expired");

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

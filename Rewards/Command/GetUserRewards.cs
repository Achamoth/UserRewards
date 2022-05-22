using Rewards.Contracts.Request;
using Rewards.Contracts.Response;
using Rewards.Data;
using Rewards.Domain;
using Rewards.Helpers;
using Rewards.Middleware;
using Rewards.Services;

namespace Rewards.Command
{
    public class GetUserRewards : ICommand<UserRewardResponse>
    {
        private readonly IStorageProvider _storageProvider;
        private readonly IDateTimeService _dateTimeService;

        private int _userId;
        private UserRewardRequest _request;

        public GetUserRewards(IStorageProvider storageProvider, IDateTimeService dateTimeService)
        {
            _storageProvider = storageProvider;
            _dateTimeService = dateTimeService;
        }

        public void SetParameters(int id, UserRewardRequest request)
        {
            _userId = id;
            _request = request;
        }

        public async Task<UserRewardResponse> ExecuteAsync()
        {
            var result = new UserRewardResponse { Data = new List<UserRewardResponse.Reward>() };

            // Assume we want to use current week if none provided
            var startOfWeek = _request.At?.GetStartOfWeek() ?? _dateTimeService.Now.GetStartOfWeek();
            var endOfWeek = startOfWeek.AddDays(6);

            var user = await _storageProvider.FindUserByIdAsync(_userId);
            if (user == null)
            {
                user = new User
                {
                    Id = _userId
                };
                await _storageProvider.AddUserAsync(user);
            }

            var rewardsForWeek = user.Rewards.Where(r => r.AvailableAt >= startOfWeek && r.AvailableAt <= endOfWeek);

            if (rewardsForWeek.Any() && rewardsForWeek.Count() != 7)
                throw new HttpResponseException(StatusCodes.Status400BadRequest, "The user's rewards are invalid"); // Rudimentary error-handling; this should never happen

            if (!rewardsForWeek.Any())
            {
                for (var i = 0; i < 7; i++)
                {
                    var reward = new Reward
                    {
                        AvailableAt = startOfWeek.AddDays(i),
                        ExpiresAt = startOfWeek.AddDays(i + 1),
                        UserId = _userId
                    };
                    await _storageProvider.AddOrUpdateRewardAsync(reward);
                    result.Data.Add(MapToResponse(reward));
                }
            }
            else
            {
                foreach (var reward in rewardsForWeek.OrderBy(r => r.AvailableAt))
                {
                    result.Data.Add(MapToResponse(reward));
                }
            }

            return result;
        }

        private static UserRewardResponse.Reward MapToResponse(Reward reward)
        {
            return new UserRewardResponse.Reward
            {
                AvailableAt = reward.AvailableAt,
                ExpiresAt = reward.ExpiresAt,
                RedeemedAt = reward.RedeemedAt
            };
        }
    }
}

using Rewards.Contracts.Request;
using Rewards.Contracts.Response;
using Rewards.Data;

namespace Rewards.Command
{
    public class GetUserRewards : ICommand<UserRewardResponse>
    {
        private readonly IStorageProvider _storageProvider;

        private UserRewardRequest _request;

        public GetUserRewards(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

        public void SetParameters(UserRewardRequest request)
        {
            _request = request;
        }

        public async Task<UserRewardResponse> ExecuteAsync()
        {
            await Task.CompletedTask;
            return new UserRewardResponse();
        }
    }
}

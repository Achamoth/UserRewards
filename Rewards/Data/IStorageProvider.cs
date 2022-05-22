using Rewards.Domain;

namespace Rewards.Data
{
    public interface IStorageProvider
    {
        public Task AddUserAsync(User user);
        public Task AddOrUpdateRewardAsync(Reward reward);
        public Task<User> FindUserByIdAsync(int id);
    }
}

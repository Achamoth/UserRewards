using Rewards.Domain;

namespace Rewards.Data
{
    public interface IStorageProvider
    {
        public User FindUser(int id);
        public void AddAsync(IEntity entity);
    }
}

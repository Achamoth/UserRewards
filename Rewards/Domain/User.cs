namespace Rewards.Domain
{
    public class User : IEntity
    {
        public int Id { get; set; }
        public List<Reward> Rewards { get; set; }

        public User()
        {
            Rewards = new List<Reward>();
        }
    }
}

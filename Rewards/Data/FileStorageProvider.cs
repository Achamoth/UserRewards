using Rewards.Domain;

namespace Rewards.Data
{
    public class FileStorageProvider : IStorageProvider
    {
        public async Task AddUserAsync(User user)
        {
            // Rudimentary; will overwrite existing user
            await Task.CompletedTask;
            File.WriteAllText($"users/{user.Id}.txt", "");
        }

        public async Task AddRewardAsync(Reward reward)
        {
            // Rudimentary; doesn't check if reward already exists
            await Task.CompletedTask;
            File.AppendAllText(
                $"users/{reward.UserId}.txt",
                $"{reward.AvailableAt},{reward.RedeemedAt.ToString() ?? ""},{reward.ExpiresAt}\n");
        }

        public async Task<User> FindUserByIdAsync(int id)
        {
            var filename = $"users/{id}.txt";
            if (!File.Exists(filename))
                return null;

            var lines = await File.ReadAllLinesAsync(filename);
            var user = new User { Id = id };
            foreach (var line in lines)
            {
                // Rudimentary; not handling parsing errors here
                var tokens = line.Split(",");
                user.Rewards.Add(new Reward
                {
                    AvailableAt = DateTime.Parse(tokens[0].Trim()),
                    ExpiresAt = DateTime.Parse(tokens[2].Trim()),
                    RedeemedAt = string.IsNullOrEmpty(tokens[1].Trim()) ? null : (DateTime?)DateTime.Parse(tokens[1].Trim())
                });
            }
            return user;
        }
    }
}

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

        public async Task AddOrUpdateRewardAsync(Reward reward)
        {
            // Not efficient
            await Task.CompletedTask;
            var newRewardLine = $"{reward.AvailableAt},{reward.RedeemedAt.ToString() ?? ""},{reward.ExpiresAt}";
            var filepath = $"users/{reward.UserId}.txt";
            var modifiedLine = false;

            if (File.Exists(filepath))
            {
                var lines = File.ReadAllLines(filepath).ToList();
                for (var i = 0; i < lines.Count; i++)
                {
                    var line = lines[i].Trim();
                    if (line.StartsWith(reward.AvailableAt.ToString()))
                    {
                        lines[i] = newRewardLine;
                        modifiedLine = true;
                        break;
                    }
                }
                if (!modifiedLine)
                    lines.Add(newRewardLine);
                File.WriteAllLines(filepath, lines);
            }
            else
            {
                File.WriteAllText(filepath, newRewardLine);
            }
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
                    RedeemedAt = string.IsNullOrEmpty(tokens[1].Trim()) ? null : DateTime.Parse(tokens[1].Trim()),
                    UserId = id
                });
            }
            return user;
        }
    }
}

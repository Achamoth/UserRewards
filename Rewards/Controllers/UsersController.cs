using Microsoft.AspNetCore.Mvc;
using Rewards.Command;
using Rewards.Contracts.Request;
using Rewards.Contracts.Response;

namespace Rewards.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly ICommandExecutor _commandExecutor;

        public UsersController(ICommandExecutor commandExecutor)
        {
            _commandExecutor = commandExecutor;
        }

        /// <summary>
        /// GET User rewards for a specified week
        /// </summary>
        [Route("{id}/rewards")]
        [HttpGet]
        public async Task<UserRewardResponse> GetUserRewards(int id, [FromQuery] UserRewardRequest request)
        {
            return await _commandExecutor.ExecuteCommandAsync<GetUserRewards, UserRewardResponse>(c => c.SetParameters(id, request));
        }

        [Route("{id}/rewards/{availableAt}/redeem")]
        [HttpPatch]
        public async Task<RedeemRewardResponse> RedeemUserReward(int id, DateTime availableAt)
        {
            return await _commandExecutor.ExecuteCommandAsync<RedeemReward, RedeemRewardResponse>(c => c.SetParameters(id, availableAt));
        }
    }
}
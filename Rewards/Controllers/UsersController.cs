using Microsoft.AspNetCore.Mvc;
using Rewards.Contracts.Request;
using Rewards.Contracts.Response;

namespace Rewards.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        [HttpGet(Name = "GetUserRewards")]
        [Route("/{id}/rewards")]
        public UserRewardResponse GetUserRewards(int id, [FromQuery] UserRewardRequest request)
        {
            return new UserRewardResponse();
        }
    }
}
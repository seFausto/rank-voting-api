using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RankVotingApi.Votes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RankVotingApi.Controllers
{
    [ApiController]
    [EnableCors()]
    [Route("[controller]")]
    public class VoteController : ControllerBase
    {
        private readonly IVoteBusiness voteBusiness;
        public VoteController(IVoteBusiness voteBusiness)
        {
            this.voteBusiness = voteBusiness;
        }

        [HttpPost("{voteId}/submit/{userId}")]
        public async Task<IActionResult> SubmitVote(string voteId, string userId,
            [FromBody] IEnumerable<string> ranking)
        {
            if (await voteBusiness.SaveVotes(voteId, userId, ranking))
                return new OkObjectResult(JsonConvert.SerializeObject("success"));
            else
                return StatusCode(500, JsonConvert.SerializeObject("failed"));
        }

        [HttpPost("{voteId}/candidates/{didVote}")]
        public async Task<IActionResult> GetCandidates(string voteId, bool didVote,
            [FromBody] string userId)
        {
            IEnumerable<string> candidates;

            if (didVote)
            {
                candidates = await voteBusiness.GetSubmittedVote(voteId, userId);
            }
            else
            {
                candidates = await voteBusiness.GetCandidates(voteId);
            }

            return new OkObjectResult(JsonConvert.SerializeObject(candidates));
        }

        [HttpGet("{voteId}/result")]
        public async Task<IActionResult> GetResult(string voteId)
        {
            var candidates = await voteBusiness.GetVoteResult(voteId);
            return new OkObjectResult(JsonConvert.SerializeObject(candidates));
        }

        [HttpPost("/new/{rankingName}")]
        public async Task<IActionResult> SubmitNewRanking(string rankingName, [FromBody] IEnumerable<string> ranking)
        {
            var voteId = await voteBusiness.SubmitNewRanking(rankingName, ranking);
            return new OkObjectResult(JsonConvert.SerializeObject(voteId));
        }

        [HttpGet("{voteId}/info")]
        public async Task<IActionResult> GetRankingInfoAsync(string voteId)
        {
            var title = await voteBusiness.GetRankingInfo(voteId.Trim());
            return new OkObjectResult(JsonConvert.SerializeObject(title));
        }
    }
}

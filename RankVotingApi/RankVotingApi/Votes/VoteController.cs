using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using RankVotingApi.Votes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RankVotingApi.Controllers
{
    [ApiController]
    [EnableCors()]
    [Route("[controller]")]
    public class VoteController(IVoteBusiness voteBusiness) : ControllerBase
    {
        private readonly IVoteBusiness voteBusiness = voteBusiness;

        [HttpGet()]
        public IActionResult HealthCheck()
        {
            return Ok();
        }

        [HttpPost("{voteId}/submit/{userId}")]
        public async Task<IActionResult> SubmitVote(
            string voteId,
            string userId,
            [FromBody] IEnumerable<string> ranking)
        {
            await voteBusiness.SaveVotes(voteId, userId, ranking);
            return Ok();
        }

        [HttpPost("{voteId}/candidates/{didVote}")]
        public async Task<IActionResult> GetCandidates(string voteId, bool didVote,
            [FromBody] string userId)
        {
            if (didVote && string.IsNullOrEmpty(userId))
                return BadRequest("User ID is required when didVote is true.");

            var candidates = didVote
                ? await voteBusiness.GetSubmittedVote(voteId, userId)
                : await voteBusiness.GetCandidates(voteId);

            return Ok(new { Candidates = candidates });
        }

        [HttpGet("{voteId}/result")]
        public async Task<IActionResult> GetResult(string voteId)
        {
            var candidates = await voteBusiness.GetVoteResult(voteId);
            return Ok(candidates);
        }

        [HttpPost("new/{rankingName}")]
        public async Task<IActionResult> SubmitNewRanking(
            string rankingName,
            [FromBody] IEnumerable<string> ranking)
        {
            var voteId = await voteBusiness.SubmitNewRanking(rankingName, ranking);
            return Ok(voteId);
        }

        [HttpGet("{voteId}/info")]
        public async Task<IActionResult> GetRankingInfo(string voteId)
        {
            var title = await voteBusiness.GetRankingInfo(voteId.Trim());
            return Ok(title);
        }
    }
}

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using RankVotingApi.Votes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

namespace RankVotingApi.Controllers
{
    [ApiController]
    [EnableCors()]
    [Route("[controller]")]
    public class VoteController(IVoteBusiness voteBusiness) : ControllerBase
    {
        private readonly IVoteBusiness voteBusiness = voteBusiness;

        [HttpGet()]
        public IActionResult HealthCheckAsync()
        {
            Console.WriteLine("Healthcheck");
            return StatusCode(200, Ok());
        }

        [HttpPost("{voteId}/submit/{userId}")]
        public async Task<IActionResult> SubmitVote(string voteId, string userId,
            [FromBody] IEnumerable<string> ranking)
        {
            if (await voteBusiness.SaveVotes(voteId, userId, ranking))
                return new OkObjectResult(SerializeJson("success"));
            else
                return StatusCode(500, SerializeJson("failed"));
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

            return new OkObjectResult(SerializeJson(new { Candidates = candidates }));
        }


        [HttpGet("{voteId}/result")]
        public async Task<IActionResult> GetResult(string voteId)
        {
            var candidates = await voteBusiness.GetVoteResult(voteId);
            return new OkObjectResult(SerializeJson(candidates));
        }

        [HttpPost("new/{rankingName}")]
        public async Task<IActionResult> SubmitNewRanking(string rankingName, [FromBody] IEnumerable<string> ranking)
        {
            var voteId = await voteBusiness.SubmitNewRanking(rankingName, ranking);
            return new OkObjectResult(SerializeJson(voteId));
        }

        [HttpGet("{voteId}/info")]
        public async Task<IActionResult> GetRankingInfoAsync(string voteId)
        {
            var title = await voteBusiness.GetRankingInfo(voteId.Trim());
            return new OkObjectResult(SerializeJson(title));
        }

        private static string SerializeJson(object candidates)
        {
            return JsonSerializer.Serialize(candidates);
        }
    }
}

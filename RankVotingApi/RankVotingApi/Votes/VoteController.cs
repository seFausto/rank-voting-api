using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RankVotingApi.Votes;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [HttpPost("{voteId}/{userId}")]
        public async Task<IActionResult> SubmitVote(string voteId, string userId,
            [FromBody] IEnumerable<string> ranking)
        {
            if (await voteBusiness.SaveVotes(voteId, userId, ranking))
                return new OkObjectResult(JsonConvert.SerializeObject("success"));
            else
                return StatusCode(500, JsonConvert.SerializeObject("failed"));
        }

        [HttpPost("{voteId}/candidates")]
        public async Task<IActionResult> GetCandidates(string voteId, [FromBody]  string userId)
        {
            IEnumerable<string> candidates;

            if (string.IsNullOrEmpty(userId))
            {
                candidates = await voteBusiness.GetCandidates(voteId);
            }
            else
            {
                candidates = await voteBusiness.GetSubmittedRanking(voteId, userId);
            }

            return new OkObjectResult(JsonConvert.SerializeObject(candidates));
        }

        [HttpGet("{voteId}/result")]
        public async Task<IActionResult> GetResult(string voteId)
        {
            var candidates = await voteBusiness.GetVoteResult(voteId);
            return new OkObjectResult(JsonConvert.SerializeObject(candidates));
        }

        [HttpPost("/new")]
        public async Task<IActionResult> SubmitNewRanking([FromBody] IEnumerable<string> ranking)
        {
            var voteId = await voteBusiness.SubmitNewRanking(ranking);
            return new OkObjectResult(JsonConvert.SerializeObject(voteId));

        }
    }
}

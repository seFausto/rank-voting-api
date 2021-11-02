using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

        [HttpPost("{voteId}")]
        public async Task<IActionResult> SubmitVote(string voteId,
            [FromBody] IEnumerable<string> ranking)
        {
            await voteBusiness.SaveVotes(voteId, ranking);

            return await Task.FromResult(Ok());
        }

        [HttpGet("{voteId}/candidates")]
        public async Task<IActionResult> GetCandidates(string voteId)
        {
            var candidates = await voteBusiness.GetCandidates(voteId);
            return new OkObjectResult(candidates);
        }

        [HttpGet("{voteId}/result")]
        public async Task<IActionResult> GetResult(string voteId)
        {
            var candidates = await voteBusiness.GetVoteResult(voteId);
            return new OkObjectResult(candidates);
        }

        [HttpPost("/new")]
        public async Task<IActionResult> SubmitNewRanking([FromBody] IEnumerable<string> ranking)
        {
            var voteId = await voteBusiness.SubmitNewRanking(ranking);
            return new OkObjectResult(Newtonsoft.Json.JsonConvert.SerializeObject(voteId));

        }
    }
}

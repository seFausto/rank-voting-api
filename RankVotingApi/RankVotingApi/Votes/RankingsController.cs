using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RankVotingApi.Votes;

[ApiController]
[EnableCors()]
[Route("[controller]")]
public class RankingsController(IVoteBusiness voteBusiness) : ControllerBase
{
    private readonly IVoteBusiness voteBusiness = voteBusiness;

    [HttpPost("{name}")]
    public async Task<IActionResult> CreateRanking(
        string name,
        [FromBody] IEnumerable<string> candidates)
    {
        var id = await voteBusiness.SubmitNewRanking(name, candidates);
        return Ok(id);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRanking(string id)
    {
        var title = await voteBusiness.GetRankingInfo(id.Trim());
        return Ok(title);
    }

    [HttpGet("{id}/candidates")]
    public async Task<IActionResult> GetCandidates(string id)
    {
        var candidates = await voteBusiness.GetCandidates(id);
        return Ok(new { Candidates = candidates });
    }

    [HttpGet("{id}/results")]
    public async Task<IActionResult> GetResults(string id)
    {
        var candidates = await voteBusiness.GetVoteResult(id);
        return Ok(candidates);
    }

    [HttpGet("{id}/ballots/count")]
    public async Task<IActionResult> GetBallotCount(string id)
    {
        var count = await voteBusiness.GetBallotCount(id);
        return Ok(new { Count = count });
    }

    [HttpGet("{id}/ballots/{userId}")]
    public async Task<IActionResult> GetBallot(string id, string userId)
    {
        var candidates = await voteBusiness.GetSubmittedVote(id, userId);
        return Ok(new { Candidates = candidates });
    }

    [HttpPost("{id}/ballots/{userId}")]
    public async Task<IActionResult> SubmitBallot(
        string id,
        string userId,
        [FromBody] IEnumerable<string> ranking)
    {
        await voteBusiness.SaveVotes(id, userId, ranking);
        return Ok();
    }
}

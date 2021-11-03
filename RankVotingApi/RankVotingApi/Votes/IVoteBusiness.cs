using System.Collections.Generic;
using System.Threading.Tasks;

namespace RankVotingApi.Votes
{
    public interface IVoteBusiness
    {
        Task<bool> SaveVotes(string voteId, string userId, IEnumerable<string> vote);
        Task<IEnumerable<string>> GetCandidates(string voteId);
        Task<IEnumerable<string>> GetVoteResult(string voteId);
        Task<string> SubmitNewRanking(IEnumerable<string> ranking);
        Task<IEnumerable<string>> GetSubmittedVote(string voteId, string userId);
    }
}
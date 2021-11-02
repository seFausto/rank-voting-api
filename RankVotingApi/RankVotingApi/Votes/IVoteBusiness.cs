using System.Collections.Generic;
using System.Threading.Tasks;

namespace RankVotingApi.Votes
{
    public interface IVoteBusiness
    {
        Task<bool> SaveVotes(string id, IEnumerable<string> vote);
        Task<IEnumerable<string>> GetCandidates(string voteId);
        Task<IEnumerable<string>> GetVoteResult(string voteId);
        Task<string> SubmitNewRanking(IEnumerable<string> ranking);
    }
}
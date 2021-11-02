using System.Collections.Generic;
using System.Threading.Tasks;

namespace RankVotingApi.Repository
{
    public interface IVoteRepository
    {
        Task<bool> SaveVotes(string id, IEnumerable<string> rankings);
        Task<IEnumerable<string>> GetCandidates(string voteId);
        Task<IEnumerable<string>> GetVoteResult(string voteId);
        Task SubmitNewRanking(string voteId, IEnumerable<string> ranking);
    }
}
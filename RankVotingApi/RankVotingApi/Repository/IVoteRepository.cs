using System.Collections.Generic;
using System.Threading.Tasks;

namespace RankVotingApi.Repository
{
    public interface IVoteRepository
    {
        Task SaveVote(string voteId, string userId, IEnumerable<string> vote);
        Task<IEnumerable<string>> GetCandidates(string voteId);
        Task<IEnumerable<string>> GetVoteResult(string voteId);
        Task<IEnumerable<string>> GetSubmittedVote(string voteId, string userId);
        Task SubmitNewRanking(string voteId, string rankingName, IEnumerable<string> ranking);
        Task<string> GetRankingInfo(string voteId);
        Task<int> GetBallotCount(string voteId);
    }
}

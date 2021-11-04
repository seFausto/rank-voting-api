using System.Collections.Generic;
using System.Threading.Tasks;

namespace RankVotingApi.Repository
{
    public interface IVoteRepository
    {
        Task<bool> AddVote(string id, IEnumerable<string> rankings);
        Task<IEnumerable<string>> GetCandidates(string voteId);
        Task<IEnumerable<string>> GetVoteResult(string voteId);
        Task SubmitNewRanking(string voteId, string rankingName, IEnumerable<string> ranking);
        Task<IEnumerable<string>> GetSubmittedVote(string voteId, string userId);
        Task<bool> SaveVote(string voteId, string userId, IEnumerable<string> vote);

        Task<string> GetRankingInfo(string voteId);
    }
}
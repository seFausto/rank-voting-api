using RankVotingApi.Common;
using RankVotingApi.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RankVotingApi.Votes
{
    public class VoteBusiness : IVoteBusiness
    {
        private readonly IVoteRepository voteRepository;
        public VoteBusiness(IVoteRepository voteRepository)
        {
            this.voteRepository = voteRepository;
        }
        public async Task SaveVotes(string id, IEnumerable<string> vote)
        {
           await voteRepository.SaveVotes(id, vote);
        }

        public async Task<IEnumerable<string>> GetCandidates(string voteId)
        {
            var candidates = await voteRepository.GetCandidates(voteId);
            return candidates.ToList().Shuffle();
        }

        public async Task<IEnumerable<string>> GetVoteResult(string voteId)
        {
            return await voteRepository.GetVoteResult(voteId);
        }
    }
}

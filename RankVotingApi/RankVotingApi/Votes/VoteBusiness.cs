using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RankVotingApi.Common;
using RankVotingApi.Repository;

namespace RankVotingApi.Votes
{
    public class VoteBusiness : IVoteBusiness
    {
        private readonly IVoteRepository voteRepository;
        public VoteBusiness(IVoteRepository voteRepository)
        {
            this.voteRepository = voteRepository;
        }
        public async Task<bool> SaveVotes(string id, string userId, IEnumerable<string> vote)
        {
            //Also save how this user voted
            return await voteRepository.SaveVotes(id, vote);
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
        public async Task<string> SubmitNewRanking(IEnumerable<string> ranking)
        {
            //generate id
            var guid = Guid.NewGuid().ToString();
            var voteId = guid.Substring(guid.Length - 4);
            await voteRepository.SubmitNewRanking(voteId, ranking);
            return voteId;
        }

        public async Task<IEnumerable<string>> GetSubmittedRanking(string voteId, string userId)
        {
            return await voteRepository.GetSubmittedRanking(voteId, userId);
        }
    }
}

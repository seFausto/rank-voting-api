using RankVotingApi.Common;
using RankVotingApi.Repository;
using System;
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
        public async Task<bool> SaveVotes(string voteId, string userId, IEnumerable<string> vote)
        {
            //Also save how this user voted
            try
            {
                await voteRepository.SaveVote(voteId, userId, vote);
                return await voteRepository.AddVote(voteId, vote);
            }
            catch (Exception ex)
            {
                throw;
            }

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
        public async Task<string> SubmitNewRanking(string rankingName,
            IEnumerable<string> ranking)
        {
            var guid = Guid.NewGuid().ToString();
            var voteId = guid.Substring(guid.Length - 4);
            await voteRepository.SubmitNewRanking(voteId, rankingName, ranking);
            return voteId;
        }

        public async Task<IEnumerable<string>> GetSubmittedVote(string voteId, string userId)
        {
            return await voteRepository.GetSubmittedVote(voteId, userId);
        }

        public async Task<string> GetRankingInfo(string voteId)
            => await voteRepository.GetRankingInfo(voteId);
    }
}

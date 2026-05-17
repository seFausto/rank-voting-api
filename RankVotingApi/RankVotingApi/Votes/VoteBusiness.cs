using Microsoft.Extensions.Logging;
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
		private readonly ILogger<VoteBusiness> logger;

		public VoteBusiness(
            IVoteRepository voteRepository,
            ILogger<VoteBusiness> logger)
		{
			this.voteRepository = voteRepository;
			this.logger = logger;
		}

		public async Task SaveVotes(
            string voteId,
            string userId,
            IEnumerable<string> vote)
        {
            try
            {
                await voteRepository.SaveVote(voteId, userId, vote);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error when saving vote {VoteId}", voteId);
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetCandidates(string voteId)
        {
            var candidates = await voteRepository.GetCandidates(voteId);
            return candidates.ToList().Shuffle();
        }

        public async Task<IEnumerable<string>> GetVoteResult(string voteId)
            => await voteRepository.GetVoteResult(voteId);

        public async Task<string> SubmitNewRanking(
            string rankingName,
            IEnumerable<string> ranking)
        {
            var rankId = Guid.NewGuid().ToString()[..8];
            await voteRepository.SubmitNewRanking(rankId, rankingName, ranking);
            return rankId;
        }

        public async Task<IEnumerable<string>> GetSubmittedVote(
            string voteId,
            string userId)
            => await voteRepository.GetSubmittedVote(voteId, userId);

        public async Task<string> GetRankingInfo(string voteId)
            => await voteRepository.GetRankingInfo(voteId);

        public async Task<int> GetBallotCount(string voteId)
            => await voteRepository.GetBallotCount(voteId);
    }
}

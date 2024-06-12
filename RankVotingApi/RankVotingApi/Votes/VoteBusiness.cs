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
        private readonly ILogger _logger;
        public VoteBusiness(IVoteRepository voteRepository, ILogger<VoteBusiness> logger)
        {
            this.voteRepository = voteRepository;
            _logger = logger;
        }
        public async Task<bool> SaveVotes(string voteId, string userId, IEnumerable<string> vote)
        {
            try
            {
                await voteRepository.SaveVote(voteId, userId, vote);
                return await voteRepository.AddVote(voteId, vote);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                _logger.LogError(ex, "Error when saving vote {VoteId}", voteId);
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
            var rankId = guid[^4..];
            return await SubmitNewRanking(rankingName, rankId, ranking);
        }

        public async Task<string> SubmitNewRanking(string rankingName, string rankId, IEnumerable<string> ranking)
        {
            await voteRepository.SubmitNewRanking(rankId, rankingName, ranking);
            return rankId;
        }

        public async Task<IEnumerable<string>> GetSubmittedVote(string voteId, string userId)
        {
            return await voteRepository.GetSubmittedVote(voteId, userId);
        }

        public async Task<string> GetRankingInfo(string voteId)
            => await voteRepository.GetRankingInfo(voteId);
    }
}

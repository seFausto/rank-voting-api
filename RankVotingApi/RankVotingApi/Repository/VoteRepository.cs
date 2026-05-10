using Dapper;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RankVotingApi.Repository
{
    public class VoteRepository : IVoteRepository
    {
        private const string ConnectionString = "Data Source=RankChoiceVoting.db";

        public async Task<IEnumerable<string>> GetCandidates(string voteId)
        {
            const string sql = @"SELECT Candidate
                                FROM Candidates
                                WHERE VoteId = @voteId;";

            using var connection = new SqliteConnection(ConnectionString);
            return await connection.QueryAsync<string>(sql, new { voteId });
        }

        public async Task<IEnumerable<string>> GetSubmittedVote(string voteId, string userId)
        {
            const string sql = @"SELECT Candidate
                                FROM UserVotes
                                WHERE VoteId = @voteId
                                AND UserId = @userId
                                ORDER BY Rank asc";

            using var connection = new SqliteConnection(ConnectionString);
            return await connection.QueryAsync<string>(sql, new { voteId, userId });
        }

        public async Task<IEnumerable<string>> GetVoteResult(string voteId)
        {
            const string sql = @"SELECT Candidate
                                FROM Candidates
                                WHERE VoteId = @voteId
                                ORDER BY Rank ASC";

            using var connection = new SqliteConnection(ConnectionString);
            return await connection.QueryAsync<string>(sql, new { voteId });
        }

        public async Task SaveVote(string voteId, string userId, IEnumerable<string> vote)
        {
            const string insertUserVote = @"INSERT INTO UserVotes (VoteId, UserId, Rank, Candidate)
                                VALUES (@voteId, @userId, @rank, @candidate)";

            const string updateScore = @"UPDATE Candidates
                                SET Rank = Rank + @rank
                                WHERE VoteId = @voteId
                                AND Candidate = @candidate;";

            var candidates = vote.ToList();

            using var connection = new SqliteConnection(ConnectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            for (int i = 0; i < candidates.Count; i++)
            {
                await connection.ExecuteAsync(insertUserVote,
                    new { voteId, userId, rank = i, candidate = candidates[i] },
                    transaction);

                await connection.ExecuteAsync(updateScore,
                    new { rank = i, voteId, candidate = candidates[i] },
                    transaction);
            }

            await transaction.CommitAsync();
        }

        public async Task SubmitNewRanking(string voteId, string rankingName, IEnumerable<string> ranking)
        {
            const string insertIntoRanking = @"INSERT INTO Ranking (VoteId, Title, Description)
                                 VALUES (@voteId, @title, @description)";

            const string insertIntoCandidates = @"INSERT INTO Candidates (VoteId, Candidate, Rank)
                                 VALUES (@voteId, @candidate, @rank)";

            var candidates = ranking.ToList();

            using var connection = new SqliteConnection(ConnectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            await connection.ExecuteAsync(insertIntoRanking,
                new { voteId, title = rankingName, description = string.Empty },
                transaction);

            for (int i = 0; i < candidates.Count; i++)
            {
                await connection.ExecuteAsync(insertIntoCandidates,
                    new { rank = 0, voteId, candidate = candidates[i] },
                    transaction);
            }

            await transaction.CommitAsync();
        }

        public async Task<string> GetRankingInfo(string voteId)
        {
            const string sql = @"SELECT Title
                                FROM Ranking
                                WHERE VoteId = @voteId";

            using var connection = new SqliteConnection(ConnectionString);
            return await connection.QuerySingleOrDefaultAsync<string>(sql, new { voteId });
        }
    }
}

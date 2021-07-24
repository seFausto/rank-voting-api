using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;

namespace RankVotingApi.Repository
{
    public class VoteRepository : IVoteRepository
    {
        public async Task<IEnumerable<string>> GetCandidates(string voteId)
        {
            const string sql = @"SELECT Candidate 
                                FROM Candidates 
                                WHERE VoteId = @voteId;";

            using var connection = new SqliteConnection("Data Source=RankChoiceVoting.db");

            return await connection.QueryAsync<string>(sql,
                new
                {
                    voteId
                });
        }

        public async Task<IEnumerable<string>> GetVoteResult(string voteId)
        {
            const string sql = @"SELECT Candidate 
                                FROM Candidates 
                                WHERE VoteId = @voteId
                                ORDER BY Rank ASC";

            using var connection = new SqliteConnection("Data Source=RankChoiceVoting.db");

            return await connection.QueryAsync<string>(sql,
                new
                {
                    voteId
                });
        }

        public async Task SaveVotes(string id, IEnumerable<string> rankings)
        {
            const string sql = @"UPDATE Candidates
                                SET Rank = Rank + @rank
                                WHERE VoteId = @voteId
                                AND Candidate = @candidate;";

            using var connection = new SqliteConnection("Data Source=RankChoiceVoting.db");

            for (int index = 0; index < rankings.Count(); index++)
            {
                await connection.ExecuteAsync(sql,
                    new
                    {
                        rank = index,
                        voteId = id,
                        candidate = rankings.ElementAt(index)
                    });
            }
        }

        public async Task SubmitNewRanking(string voteId, IEnumerable<string> ranking)
        {
            const string sql = @"INSERT INTO Candidates (VoteId, Candidate, Rank)
                                 VALUES (voteId, candidate, rank)   ";

                                   using var connection = new SqliteConnection("Data Source=RankChoiceVoting.db");

            
            for (int index = 0; index < ranking.Count(); index++)
            {
                await connection.ExecuteAsync(sql,
                    new
                    {
                        rank = 0,
                        voteId = voteId,
                        candidate = ranking.ElementAt(index)
                    });
            }

        }
    }
}

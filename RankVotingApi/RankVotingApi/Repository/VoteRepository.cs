using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            //foreach value of ranking add the number to the current one
            //update by candidate and voteid
            //and do rank = rank + index ranking element

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
    }
}

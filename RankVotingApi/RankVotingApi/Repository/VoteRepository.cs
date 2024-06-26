﻿using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
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

        public async Task<IEnumerable<string>> GetSubmittedVote(string voteId, string userId)
        {
            const string sql = @"SELECT Candidate 
                                FROM UserVotes
                                WHERE VoteId = @voteId
                                AND UserId = @userId
                                ORDER BY Rank asc";

            using var connection = new SqliteConnection("Data Source=RankChoiceVoting.db");

            try
            {
                return await connection.QueryAsync<string>(sql,
                new
                {
                    voteId,
                    userId
                });
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                throw;
            }

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

        public async Task<bool> AddVote(string id, IEnumerable<string> rankings)
        {
            const string sql = @"UPDATE Candidates
                                SET Rank = Rank + @rank
                                WHERE VoteId = @voteId
                                AND Candidate = @candidate;";

            using var connection = new SqliteConnection("Data Source=RankChoiceVoting.db");
            try
            {
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

                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                throw;
            }
        }

        public async Task SubmitNewRanking(string voteId, string rankingName, IEnumerable<string> ranking)
        {
            const string insertIntoCandidates = @"INSERT INTO Candidates (VoteId, Candidate, Rank)
                                 VALUES (@voteId, @candidate, @rank)";

            const string insertIntoRanking = @"INSERT INTO Ranking (VoteId, Title, Description) 
                                 VALUES (@voteId, @title, @description)";

            using var connection = new SqliteConnection("Data Source=RankChoiceVoting.db");

            try
            {
                await connection.ExecuteAsync(insertIntoRanking,
                    new
                    {
                        voteId,
                        title = rankingName,
                        description = string.Empty
                    });

                for (int index = 0; index < ranking.Count(); index++)
                {
                    await connection.ExecuteAsync(insertIntoCandidates,
                        new
                        {
                            rank = 0,
                            voteId,
                            candidate = ranking.ElementAt(index)
                        });
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                throw;
            }
        }

        public async Task<bool> SaveVote(string voteId, string userId, IEnumerable<string> vote)
        {
            const string sql = @"INSERT INTO UserVotes (VoteId, UserId, Rank, Candidate) 
                                VALUES (@voteId, @userId, @rank, @candidate)";

            using var connection = new SqliteConnection("Data Source=RankChoiceVoting.db");
            try
            {
                for (int index = 0; index < vote.Count(); index++)
                {
                    await connection.ExecuteAsync(sql,
                        new
                        {
                            voteId,
                            userId,
                            rank = index,
                            candidate = vote.ElementAt(index)
                        });
                }

                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                throw;
            }
        }

        public async Task<string> GetRankingInfo(string voteId)
        {
            const string sql = @"SELECT Title 
                                FROM Ranking 
                                WHERE VoteId = @voteId";

            try
            {
                using var connection = new SqliteConnection("Data Source=RankChoiceVoting.db");
                var title = await connection.QuerySingleOrDefaultAsync<string>(sql,
                    new
                    {
                        voteId
                    });
                return title;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                throw;
            }

        }
    }
}

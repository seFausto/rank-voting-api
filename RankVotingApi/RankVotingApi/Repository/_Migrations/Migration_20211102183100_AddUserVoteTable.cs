using FluentMigrator;

namespace RankVotingApi
{
    [Migration(2021110218310)]
    public class Migration_20211102183100_AddUserVoteTable : Migration
    {
        public override void Down()
        {
            Delete.Table("UserVotes");
        }

        public override void Up()
        {
            Create.Table("UserVotes")
               .WithColumn("Id").AsInt64().PrimaryKey().Identity()
               .WithColumn("VoteId").AsString()
               .WithColumn("UserId").AsString()
               .WithColumn("Rank").AsInt32()
               .WithColumn("Candidate").AsString();
        }
    }
}

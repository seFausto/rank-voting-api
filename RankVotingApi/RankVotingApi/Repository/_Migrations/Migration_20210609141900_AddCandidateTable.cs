using FluentMigrator;

namespace RankVotingApi
{
    [Migration(20210609141900)]
    public class Migration_20210609141900_AddCandidateTable : Migration
    {
        public override void Down()
        {
            Delete.Table("Candidates");
        }

        public override void Up()
        {
            Create.Table("Candidates")
               .WithColumn("Id").AsInt64().PrimaryKey().Identity()
               .WithColumn("VoteId").AsString()
               .WithColumn("Candidate").AsString()
               .WithColumn("Rank").AsInt32();

            Insert.IntoTable("Candidates").Row(new { VoteId = "1234", Candidate = "Kevin", Rank = 0 });
            Insert.IntoTable("Candidates").Row(new { VoteId = "1234", Candidate = "Randall", Rank = 0 });
            Insert.IntoTable("Candidates").Row(new { VoteId = "1234", Candidate = "Kate", Rank = 0 });
            Insert.IntoTable("Candidates").Row(new { VoteId = "1234", Candidate = "Rebecca", Rank = 0 });
            Insert.IntoTable("Candidates").Row(new { VoteId = "1234", Candidate = "Jack", Rank = 0 });
            Insert.IntoTable("Candidates").Row(new { VoteId = "1234", Candidate = "Beth", Rank = 0 });
            Insert.IntoTable("Candidates").Row(new { VoteId = "1234", Candidate = "Toby", Rank = 0 });
            Insert.IntoTable("Candidates").Row(new { VoteId = "1234", Candidate = "Miguel", Rank = 0 });
            Insert.IntoTable("Candidates").Row(new { VoteId = "1234", Candidate = "Madison", Rank = 0 });
        }
    }
}

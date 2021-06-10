using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RankVotingApi
{
    [Migration(20210609141600)]
    public class Migration_20210609141600_AddVotingTable : Migration
    {
        public override void Down()
        {
            Delete.Table("Voting");
        }

        public override void Up()
        {
            Create.Table("Voting")
               .WithColumn("Id").AsInt64().PrimaryKey().Identity()
               .WithColumn("VoteId").AsString().Unique()
               .WithColumn("Title").AsString()
               .WithColumn("Description").AsString();

            Insert.IntoTable("Voting").Row(new
            {
                VoteId = "1234",
                Title = "Favorite 'This Is Us' Characters",
                Description = "This is a test run of Rank Choice voting"
            });
        }
    }
}

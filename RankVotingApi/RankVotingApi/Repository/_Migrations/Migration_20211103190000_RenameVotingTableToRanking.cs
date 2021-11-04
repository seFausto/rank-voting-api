using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RankVotingApi
{
    [Migration(20211103190000)]
    public class Migration_20211103190000_RenameVotingTableToRanking : Migration
    {
        public override void Down()
        {
            Rename.Table("Ranking").To("Voting");
        }

        public override void Up()
        {

            Rename.Table("Voting").To("Ranking");
        }
    }
}

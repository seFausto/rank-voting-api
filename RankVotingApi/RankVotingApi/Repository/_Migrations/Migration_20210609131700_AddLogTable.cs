using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RankVotingApi
{
    [Migration(20210609131700)]
    public class Migration_20210609131700_AddLogTable : Migration
    {
        public override void Down()
        {
            Delete.Table("Log");
        }

        public override void Up()
        {
            Create.Table("Log")
               .WithColumn("Id").AsInt64().PrimaryKey().Identity()
               .WithColumn("Text").AsString();
        }
    }
}

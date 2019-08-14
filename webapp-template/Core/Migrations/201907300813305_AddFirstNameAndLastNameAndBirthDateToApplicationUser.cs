using System.Data.Entity.Migrations;

namespace Core.Migrations
{
    public partial class AddFirstNameAndLastNameAndBirthDateToApplicationUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "FirstName", c => c.String(false, 255));
            AddColumn("dbo.AspNetUsers", "LastName", c => c.String(false, 255));
            AddColumn("dbo.AspNetUsers", "BirthDate", c => c.DateTime());
        }

        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "BirthDate");
            DropColumn("dbo.AspNetUsers", "LastName");
            DropColumn("dbo.AspNetUsers", "FirstName");
        }
    }
}
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_comm_DataAccess.Migrations
{
    public partial class AddSPforCoverType : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE PROCEDURE Create_CoverType
                        @name varchar(50)
                            AS
                    insert CoverTypes values(@name)");
            migrationBuilder.Sql(@"CREATE PROCEDURE Update_CoverType
                        @Id int,
                        @name varchar(50)
                        AS
                        update CoverTypes set name=@name where Id=@Id");
            migrationBuilder.Sql(@"CREATE PROCEDURE Delete_CoverType
                        @id int
                        AS
                        delete from CoverTypes");
            migrationBuilder.Sql(@"CREATE PROCEDURE Get_CoverTypes
                                As
                                select * from CoverTypes");
            migrationBuilder.Sql(@"CREATE PROCEDURE Get_CoverType
                                @id int
                                AS
                                select * from CoverTypes where Id=@Id");
                                }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

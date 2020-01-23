using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookShelf.Migrations
{
    public partial class Comments_In_Books : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Comment",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "00000000-ffff-ffff-ffff-ffffffffffff",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "113abda9-afdd-4228-b46f-05a365bf6c63", "AQAAAAEAACcQAAAAEKcwnly4OOTW9pXqmZH5sWUGlVswgx9ESzwYiOMeO3hhGrMQs4XhnzNSxNAD0uRnRw==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "Comment");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "00000000-ffff-ffff-ffff-ffffffffffff",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "14865227-7341-4f6d-ad8d-468b66db4a94", "AQAAAAEAACcQAAAAEB3YNIe1MOyU/EYUEtxh/Kg7ghBvqxLIZGiqTqutCJaPBBn3/SPFBNQ0gZfKeM2z0w==" });
        }
    }
}

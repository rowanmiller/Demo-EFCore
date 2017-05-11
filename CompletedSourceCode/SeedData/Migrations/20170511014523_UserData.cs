using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SeedData.Migrations
{
    public partial class UserData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Insert(
                table: "User",
                columns: new[] { "UserId", "UserName" },
                values: new object[] { 1, "guest" });

            migrationBuilder.Insert(
                table: "User",
                columns: new[] { "UserId", "UserName" },
                values: new object[] { 2, "administrator" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Delete(
                table: "User",
                keyColumns: new[] { "UserId" },
                keyValues: new object[] { 1 });

            migrationBuilder.Delete(
                table: "User",
                keyColumns: new[] { "UserId" },
                keyValues: new object[] { 2 });
        }
    }
}

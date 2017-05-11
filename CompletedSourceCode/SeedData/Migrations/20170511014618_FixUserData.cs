using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SeedData.Migrations
{
    public partial class FixUserData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Delete(
                table: "User",
                keyColumns: new[] { "UserId" },
                keyValues: new object[] { 1 });

            migrationBuilder.Update(
                table: "User",
                keyColumns: new[] { "UserId" },
                keyValues: new object[] { 2 },
                columns: new[] { "UserName" },
                values: new object[] { "Administrator" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Update(
                table: "User",
                keyColumns: new[] { "UserId" },
                keyValues: new object[] { 2 },
                columns: new[] { "UserName" },
                values: new object[] { "administrator" });

            migrationBuilder.Insert(
                table: "User",
                columns: new[] { "UserId", "UserName" },
                values: new object[] { 1, "guest" });
        }
    }
}

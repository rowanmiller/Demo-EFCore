using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SeedData.Migrations
{
    public partial class TagData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Insert(
                table: "Tags",
                columns: new[] { "TagId" },
                values: new object[] { ".NET" });

            migrationBuilder.Insert(
                table: "Tags",
                columns: new[] { "TagId" },
                values: new object[] { "VisualStudio" });

            migrationBuilder.Insert(
                table: "Tags",
                columns: new[] { "TagId" },
                values: new object[] { "C#" });

            migrationBuilder.Insert(
                table: "Tags",
                columns: new[] { "TagId" },
                values: new object[] { "F#" });

            migrationBuilder.Insert(
                table: "Tags",
                columns: new[] { "TagId" },
                values: new object[] { "VB.NET" });

            migrationBuilder.Insert(
                table: "Tags",
                columns: new[] { "TagId" },
                values: new object[] { "EnityFramework" });

            migrationBuilder.Insert(
                table: "Tags",
                columns: new[] { "TagId" },
                values: new object[] { "ASP.NET" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Delete(
                table: "Tags",
                keyColumns: new[] { "TagId" },
                keyValues: new object[] { ".NET" });

            migrationBuilder.Delete(
                table: "Tags",
                keyColumns: new[] { "TagId" },
                keyValues: new object[] { "ASP.NET" });

            migrationBuilder.Delete(
                table: "Tags",
                keyColumns: new[] { "TagId" },
                keyValues: new object[] { "C#" });

            migrationBuilder.Delete(
                table: "Tags",
                keyColumns: new[] { "TagId" },
                keyValues: new object[] { "EnityFramework" });

            migrationBuilder.Delete(
                table: "Tags",
                keyColumns: new[] { "TagId" },
                keyValues: new object[] { "F#" });

            migrationBuilder.Delete(
                table: "Tags",
                keyColumns: new[] { "TagId" },
                keyValues: new object[] { "VB.NET" });

            migrationBuilder.Delete(
                table: "Tags",
                keyColumns: new[] { "TagId" },
                keyValues: new object[] { "VisualStudio" });
        }
    }
}

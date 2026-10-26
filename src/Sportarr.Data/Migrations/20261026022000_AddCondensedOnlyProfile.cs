using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Sportarr.Api.Data;

namespace Sportarr.Api.Migrations;

public partial class AddCondensedOnlyProfile : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) =>
        migrationBuilder.AddColumn<bool>(name: "CondensedOnly", table: "QualityProfiles", type: "INTEGER", nullable: false, defaultValue: false);
    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropColumn(name: "CondensedOnly", table: "QualityProfiles");
}

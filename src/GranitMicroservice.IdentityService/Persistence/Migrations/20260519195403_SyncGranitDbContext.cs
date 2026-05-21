using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GranitMicroservice.IdentityService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SyncGranitDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_identity_user_cache_tenant_email",
                table: "identity_user_cache_entries");

            migrationBuilder.DropIndex(
                name: "ix_identity_user_cache_tenant_name",
                table: "identity_user_cache_entries");

            migrationBuilder.DropIndex(
                name: "ix_identity_user_cache_tenant_username",
                table: "identity_user_cache_entries");

            migrationBuilder.RenameColumn(
                name: "ExtraPropertiesJson",
                table: "identity_user_cache_entries",
                newName: "MetadataJson");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "identity_user_cache_entries",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "identity_user_cache_entries",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "identity_user_cache_entries",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "identity_user_cache_entries",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(512)",
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailHash",
                table: "identity_user_cache_entries",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "identity_user_cache_entries",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_identity_user_cache_tenant_email_hash",
                table: "identity_user_cache_entries",
                columns: new[] { "TenantId", "EmailHash" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_identity_user_cache_tenant_email_hash",
                table: "identity_user_cache_entries");

            migrationBuilder.DropColumn(
                name: "EmailHash",
                table: "identity_user_cache_entries");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "identity_user_cache_entries");

            migrationBuilder.RenameColumn(
                name: "MetadataJson",
                table: "identity_user_cache_entries",
                newName: "ExtraPropertiesJson");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "identity_user_cache_entries",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2048)",
                oldMaxLength: 2048,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "identity_user_cache_entries",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2048)",
                oldMaxLength: 2048,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "identity_user_cache_entries",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2048)",
                oldMaxLength: 2048,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "identity_user_cache_entries",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2048)",
                oldMaxLength: 2048,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_identity_user_cache_tenant_email",
                table: "identity_user_cache_entries",
                columns: new[] { "TenantId", "Email" });

            migrationBuilder.CreateIndex(
                name: "ix_identity_user_cache_tenant_name",
                table: "identity_user_cache_entries",
                columns: new[] { "TenantId", "LastName", "FirstName" });

            migrationBuilder.CreateIndex(
                name: "ix_identity_user_cache_tenant_username",
                table: "identity_user_cache_entries",
                columns: new[] { "TenantId", "Username" });
        }
    }
}

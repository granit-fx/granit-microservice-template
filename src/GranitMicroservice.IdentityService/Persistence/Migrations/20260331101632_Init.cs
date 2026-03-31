using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GranitMicroservice.IdentityService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "identity_user_cache_entries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalUserId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Username = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    FirstName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    LastName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    LastSyncedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExtraPropertiesJson = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_identity_user_cache_entries", x => x.Id);
                });

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

            migrationBuilder.CreateIndex(
                name: "uq_identity_user_cache_tenant_external_id",
                table: "identity_user_cache_entries",
                columns: new[] { "TenantId", "ExternalUserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "identity_user_cache_entries");
        }
    }
}

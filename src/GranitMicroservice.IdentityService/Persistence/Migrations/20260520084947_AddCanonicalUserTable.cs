using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GranitMicroservice.IdentityService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCanonicalUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "granit_identity_users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Email = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    EmailHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    FirstName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    LastName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    PhoneNumberHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    PreferredLocale = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Timezone = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_granit_identity_users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_granit_identity_users_email_hash",
                table: "granit_identity_users",
                column: "EmailHash");

            migrationBuilder.CreateIndex(
                name: "ix_granit_identity_users_tenant_id",
                table: "granit_identity_users",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "granit_identity_users");
        }
    }
}

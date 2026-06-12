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
                name: "audit_log_log_entries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CorrelationId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_log_log_entries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "identity_federated_user_cache_entries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalUserId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Username = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    Email = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    EmailHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    FirstName = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    LastName = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    LastSyncedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    MetadataJson = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_identity_federated_user_cache_entries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "identity_users",
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
                    table.PrimaryKey("PK_identity_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "audit_log_entity_changes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuditLogEntryId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    EntityId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ChangeType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_log_entity_changes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_audit_log_entity_changes_audit_log_log_entries_AuditLogEntr~",
                        column: x => x.AuditLogEntryId,
                        principalTable: "audit_log_log_entries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "audit_log_property_changes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuditEntityChangeId = table.Column<Guid>(type: "uuid", nullable: false),
                    PropertyName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    OriginalValue = table.Column<string>(type: "text", nullable: true),
                    NewValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_log_property_changes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_audit_log_property_changes_audit_log_entity_changes_AuditEn~",
                        column: x => x.AuditEntityChangeId,
                        principalTable: "audit_log_entity_changes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_audit_log_entity_changes_AuditLogEntryId",
                table: "audit_log_entity_changes",
                column: "AuditLogEntryId");

            migrationBuilder.CreateIndex(
                name: "ix_audit_log_entity_changes_type_id",
                table: "audit_log_entity_changes",
                columns: new[] { "EntityType", "EntityId", "AuditLogEntryId" });

            migrationBuilder.CreateIndex(
                name: "ix_audit_log_log_entries_tenant_timestamp",
                table: "audit_log_log_entries",
                columns: new[] { "TenantId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "ix_audit_log_log_entries_timestamp",
                table: "audit_log_log_entries",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "ix_audit_log_log_entries_user_timestamp",
                table: "audit_log_log_entries",
                columns: new[] { "UserId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_audit_log_property_changes_AuditEntityChangeId",
                table: "audit_log_property_changes",
                column: "AuditEntityChangeId");

            migrationBuilder.CreateIndex(
                name: "ix_identity_federated_user_cache_tenant_email_hash",
                table: "identity_federated_user_cache_entries",
                columns: new[] { "TenantId", "EmailHash" });

            migrationBuilder.CreateIndex(
                name: "uq_identity_federated_user_cache_tenant_external_id",
                table: "identity_federated_user_cache_entries",
                columns: new[] { "TenantId", "ExternalUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_identity_users_email_hash",
                table: "identity_users",
                column: "EmailHash");

            migrationBuilder.CreateIndex(
                name: "ix_identity_users_tenant_id",
                table: "identity_users",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_log_property_changes");

            migrationBuilder.DropTable(
                name: "identity_federated_user_cache_entries");

            migrationBuilder.DropTable(
                name: "identity_users");

            migrationBuilder.DropTable(
                name: "audit_log_entity_changes");

            migrationBuilder.DropTable(
                name: "audit_log_log_entries");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GranitMicroservice.NotificationService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "notifications_delivery_attempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeliveryId = table.Column<Guid>(type: "uuid", nullable: false),
                    NotificationId = table.Column<Guid>(type: "uuid", nullable: false),
                    NotificationTypeName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ChannelName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    RecipientUserId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    OccurredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DurationMs = table.Column<long>(type: "bigint", nullable: false),
                    ErrorMessage = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    IsSuccess = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications_delivery_attempts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "notifications_mobile_push_tokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    DeviceToken = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    DeviceTokenHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Platform = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications_mobile_push_tokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "notifications_preferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    NotificationTypeName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ChannelName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications_preferences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "notifications_subscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    NotificationTypeName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    EntityType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EntityId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications_subscriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "notifications_user_notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NotificationId = table.Column<Guid>(type: "uuid", nullable: false),
                    NotificationTypeName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Severity = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RecipientUserId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Data = table.Column<string>(type: "text", nullable: false),
                    State = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ReadAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    RelatedEntityType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    RelatedEntityId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications_user_notifications", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_notifications_delivery_attempts_audit",
                table: "notifications_delivery_attempts",
                columns: new[] { "TenantId", "OccurredAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "ix_notifications_delivery_attempts_notification",
                table: "notifications_delivery_attempts",
                columns: new[] { "NotificationId", "ChannelName" });

            migrationBuilder.CreateIndex(
                name: "uq_notifications_delivery_attempts_delivery_id",
                table: "notifications_delivery_attempts",
                column: "DeliveryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_notifications_mobile_push_tokens_user_tenant",
                table: "notifications_mobile_push_tokens",
                columns: new[] { "UserId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "uq_notifications_mobile_push_tokens_device_hash_tenant",
                table: "notifications_mobile_push_tokens",
                columns: new[] { "DeviceTokenHash", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_notifications_preferences_user_type_channel_tenant",
                table: "notifications_preferences",
                columns: new[] { "UserId", "NotificationTypeName", "ChannelName", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_notifications_subscriptions_entity",
                table: "notifications_subscriptions",
                columns: new[] { "EntityType", "EntityId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "ix_notifications_subscriptions_global",
                table: "notifications_subscriptions",
                columns: new[] { "UserId", "NotificationTypeName", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "ix_notifications_user_notifications_entity_feed",
                table: "notifications_user_notifications",
                columns: new[] { "RelatedEntityType", "RelatedEntityId", "TenantId", "CreatedAt" },
                descending: new[] { false, false, false, true });

            migrationBuilder.CreateIndex(
                name: "ix_notifications_user_notifications_inbox",
                table: "notifications_user_notifications",
                columns: new[] { "RecipientUserId", "TenantId", "State", "CreatedAt" },
                descending: new[] { false, false, false, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notifications_delivery_attempts");

            migrationBuilder.DropTable(
                name: "notifications_mobile_push_tokens");

            migrationBuilder.DropTable(
                name: "notifications_preferences");

            migrationBuilder.DropTable(
                name: "notifications_subscriptions");

            migrationBuilder.DropTable(
                name: "notifications_user_notifications");
        }
    }
}

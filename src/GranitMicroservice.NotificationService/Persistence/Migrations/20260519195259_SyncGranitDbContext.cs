using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GranitMicroservice.NotificationService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SyncGranitDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "uq_notifications_mobile_push_tokens_device_tenant",
                table: "notifications_mobile_push_tokens");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceToken",
                table: "notifications_mobile_push_tokens",
                type: "character varying(4096)",
                maxLength: 4096,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(512)",
                oldMaxLength: 512);

            migrationBuilder.AddColumn<string>(
                name: "DeviceTokenHash",
                table: "notifications_mobile_push_tokens",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSuccess",
                table: "notifications_delivery_attempts",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.CreateIndex(
                name: "uq_notifications_mobile_push_tokens_device_hash_tenant",
                table: "notifications_mobile_push_tokens",
                columns: new[] { "DeviceTokenHash", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_notifications_delivery_attempts_delivery_id",
                table: "notifications_delivery_attempts",
                column: "DeliveryId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "uq_notifications_mobile_push_tokens_device_hash_tenant",
                table: "notifications_mobile_push_tokens");

            migrationBuilder.DropIndex(
                name: "uq_notifications_delivery_attempts_delivery_id",
                table: "notifications_delivery_attempts");

            migrationBuilder.DropColumn(
                name: "DeviceTokenHash",
                table: "notifications_mobile_push_tokens");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceToken",
                table: "notifications_mobile_push_tokens",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(4096)",
                oldMaxLength: 4096);

            migrationBuilder.AlterColumn<bool>(
                name: "IsSuccess",
                table: "notifications_delivery_attempts",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "uq_notifications_mobile_push_tokens_device_tenant",
                table: "notifications_mobile_push_tokens",
                columns: new[] { "DeviceToken", "TenantId" },
                unique: true);
        }
    }
}

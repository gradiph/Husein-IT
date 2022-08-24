using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessageBus.Migrations
{
    public partial class ChangeIsSentToSentAt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSent",
                table: "MessageSubscribers");

            migrationBuilder.AddColumn<DateTime>(
                name: "SentAt",
                table: "MessageSubscribers",
                type: "datetime(6)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SentAt",
                table: "MessageSubscribers");

            migrationBuilder.AddColumn<bool>(
                name: "IsSent",
                table: "MessageSubscribers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}

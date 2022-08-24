using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessageBus.Migrations
{
    public partial class ChangeChannelSubscriberRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscribers_Channels_ChannelId",
                table: "Subscribers");

            migrationBuilder.DropIndex(
                name: "IX_Subscribers_ChannelId",
                table: "Subscribers");

            migrationBuilder.DropColumn(
                name: "ChannelId",
                table: "Subscribers");

            migrationBuilder.CreateTable(
                name: "ChannelSubscriber",
                columns: table => new
                {
                    ChannelsId = table.Column<int>(type: "int", nullable: false),
                    SubscribersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelSubscriber", x => new { x.ChannelsId, x.SubscribersId });
                    table.ForeignKey(
                        name: "FK_ChannelSubscriber_Channels_ChannelsId",
                        column: x => x.ChannelsId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChannelSubscriber_Subscribers_SubscribersId",
                        column: x => x.SubscribersId,
                        principalTable: "Subscribers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelSubscriber_SubscribersId",
                table: "ChannelSubscriber",
                column: "SubscribersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChannelSubscriber");

            migrationBuilder.AddColumn<int>(
                name: "ChannelId",
                table: "Subscribers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Subscribers_ChannelId",
                table: "Subscribers",
                column: "ChannelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscribers_Channels_ChannelId",
                table: "Subscribers",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

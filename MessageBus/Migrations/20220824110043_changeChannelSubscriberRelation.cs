using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessageBus.Migrations
{
    public partial class changeChannelSubscriberRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscribers_Channels_ChannelId",
                table: "Subscribers");

            migrationBuilder.DropIndex(
                name: "IX_Subscribers_ChannelId",
                table: "Subscribers");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Subscribers",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

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
                name: "IX_Subscribers_Code",
                table: "Subscribers",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChannelSubscriber_SubscribersId",
                table: "ChannelSubscriber",
                column: "SubscribersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChannelSubscriber");

            migrationBuilder.DropIndex(
                name: "IX_Subscribers_Code",
                table: "Subscribers");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Subscribers");

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

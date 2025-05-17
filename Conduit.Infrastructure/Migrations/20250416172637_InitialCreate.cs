using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Conduit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserAccounts",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ActivationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActivationToken = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    ActivationTokenExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResetToken = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    ResetTokenExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccounts", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    LastSeen = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserAccountID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Users_UserAccounts_UserAccountID",
                        column: x => x.UserAccountID,
                        principalTable: "UserAccounts",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ChatGroups",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatorID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatGroups", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ChatGroups_Users_CreatorID",
                        column: x => x.CreatorID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "LongLivedTokens",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LongLivedTokens", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LongLivedTokens_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    SenderID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecipientID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SourceMessageID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Messages_Messages_SourceMessageID",
                        column: x => x.SourceMessageID,
                        principalTable: "Messages",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Messages_Users_RecipientID",
                        column: x => x.RecipientID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Messages_Users_SenderID",
                        column: x => x.SenderID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "SignalRClients",
                columns: table => new
                {
                    ConnectionID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignalRClients", x => new { x.ConnectionID, x.UserID });
                    table.ForeignKey(
                        name: "FK_SignalRClients_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserActivityLogs",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Agent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Origin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActivityLogs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserActivityLogs_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserID, x.Role });
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatGroupMessages",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    SenderID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChatGroupID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SourceMessageID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatGroupMessages", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ChatGroupMessages_ChatGroupMessages_SourceMessageID",
                        column: x => x.SourceMessageID,
                        principalTable: "ChatGroupMessages",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ChatGroupMessages_ChatGroups_ChatGroupID",
                        column: x => x.ChatGroupID,
                        principalTable: "ChatGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatGroupMessages_Users_SenderID",
                        column: x => x.SenderID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "ChatGroupParticipants",
                columns: table => new
                {
                    ChatGroupID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParticipantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatGroupParticipants", x => new { x.ChatGroupID, x.ParticipantID });
                    table.ForeignKey(
                        name: "FK_ChatGroupParticipants_ChatGroups_ChatGroupID",
                        column: x => x.ChatGroupID,
                        principalTable: "ChatGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatGroupParticipants_Users_ParticipantID",
                        column: x => x.ParticipantID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "ChatGroupRules",
                columns: table => new
                {
                    ChatGroupID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rule = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatGroupRules", x => new { x.ChatGroupID, x.Rule });
                    table.ForeignKey(
                        name: "FK_ChatGroupRules_ChatGroups_ChatGroupID",
                        column: x => x.ChatGroupID,
                        principalTable: "ChatGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatGroupMessages_ChatGroupID",
                table: "ChatGroupMessages",
                column: "ChatGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_ChatGroupMessages_SenderID",
                table: "ChatGroupMessages",
                column: "SenderID");

            migrationBuilder.CreateIndex(
                name: "IX_ChatGroupMessages_SourceMessageID",
                table: "ChatGroupMessages",
                column: "SourceMessageID");

            migrationBuilder.CreateIndex(
                name: "IX_ChatGroupParticipants_ParticipantID",
                table: "ChatGroupParticipants",
                column: "ParticipantID");

            migrationBuilder.CreateIndex(
                name: "IX_ChatGroups_CreatorID",
                table: "ChatGroups",
                column: "CreatorID");

            migrationBuilder.CreateIndex(
                name: "IX_LongLivedTokens_UserID",
                table: "LongLivedTokens",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RecipientID",
                table: "Messages",
                column: "RecipientID");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderID",
                table: "Messages",
                column: "SenderID");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SourceMessageID",
                table: "Messages",
                column: "SourceMessageID");

            migrationBuilder.CreateIndex(
                name: "IX_SignalRClients_UserID",
                table: "SignalRClients",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivityLogs_UserID",
                table: "UserActivityLogs",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserAccountID",
                table: "Users",
                column: "UserAccountID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatGroupMessages");

            migrationBuilder.DropTable(
                name: "ChatGroupParticipants");

            migrationBuilder.DropTable(
                name: "ChatGroupRules");

            migrationBuilder.DropTable(
                name: "LongLivedTokens");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "SignalRClients");

            migrationBuilder.DropTable(
                name: "UserActivityLogs");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "ChatGroups");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "UserAccounts");
        }
    }
}

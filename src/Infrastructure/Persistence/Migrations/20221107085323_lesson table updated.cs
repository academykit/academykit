using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class lessontableupdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_LiveSessions_live_session_id",
                table: "Meetings");

            migrationBuilder.DropTable(
                name: "LiveSessionMembers");

            migrationBuilder.DropTable(
                name: "LiveSessionModerators");

            migrationBuilder.DropTable(
                name: "LiveSessionReports");

            migrationBuilder.DropTable(
                name: "LiveSessionTags");

            migrationBuilder.DropTable(
                name: "LiveSessions");

            migrationBuilder.DropIndex(
                name: "IX_Meetings_live_session_id",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "live_session_id",
                table: "Meetings");

            migrationBuilder.AddColumn<int>(
                name: "capacity",
                table: "ZoomLicenses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "ZoomLicenses",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "start_date",
                table: "Meetings",
                type: "DATETIME",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "zoom_license_id",
                table: "Meetings",
                type: "VARCHAR(50)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "document_url",
                table: "Lessons",
                type: "VARCHAR(250)",
                maxLength: 250,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "meeting_id",
                table: "Lessons",
                type: "VARCHAR(50)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "type",
                table: "Lessons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MeetingReports",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    meeting_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    start_time = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    join_time = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    left_time = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    duration = table.Column<TimeSpan>(type: "time(6)", nullable: true),
                    created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingReports", x => x.id);
                    table.ForeignKey(
                        name: "FK_MeetingReports_Meetings_meeting_id",
                        column: x => x.meeting_id,
                        principalTable: "Meetings",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_MeetingReports_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VideoQueue",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    lesson_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    video_url = table.Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<int>(type: "int", nullable: false),
                    created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoQueue", x => x.id);
                    table.ForeignKey(
                        name: "FK_VideoQueue_Lessons_lesson_id",
                        column: x => x.lesson_id,
                        principalTable: "Lessons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_zoom_license_id",
                table: "Meetings",
                column: "zoom_license_id");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_meeting_id",
                table: "Lessons",
                column: "meeting_id");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingReports_meeting_id",
                table: "MeetingReports",
                column: "meeting_id");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingReports_user_id",
                table: "MeetingReports",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_VideoQueue_lesson_id",
                table: "VideoQueue",
                column: "lesson_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Meetings_meeting_id",
                table: "Lessons",
                column: "meeting_id",
                principalTable: "Meetings",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_ZoomLicenses_zoom_license_id",
                table: "Meetings",
                column: "zoom_license_id",
                principalTable: "ZoomLicenses",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Meetings_meeting_id",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_ZoomLicenses_zoom_license_id",
                table: "Meetings");

            migrationBuilder.DropTable(
                name: "MeetingReports");

            migrationBuilder.DropTable(
                name: "VideoQueue");

            migrationBuilder.DropIndex(
                name: "IX_Meetings_zoom_license_id",
                table: "Meetings");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_meeting_id",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "capacity",
                table: "ZoomLicenses");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "ZoomLicenses");

            migrationBuilder.DropColumn(
                name: "start_date",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "zoom_license_id",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "document_url",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "meeting_id",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "type",
                table: "Lessons");

            migrationBuilder.AddColumn<string>(
                name: "live_session_id",
                table: "Meetings",
                type: "VARCHAR(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LiveSessions",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    description = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    end_date = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    event_type = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nearest_end_time = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    nearest_start_time = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    recurrence = table.Column<string>(type: "VARCHAR(250)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    slug = table.Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    start_date = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updated_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiveSessions", x => x.id);
                    table.ForeignKey(
                        name: "FK_LiveSessions_Users_created_by",
                        column: x => x.created_by,
                        principalTable: "Users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LiveSessionMembers",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    live_session_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    updated_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiveSessionMembers", x => x.id);
                    table.ForeignKey(
                        name: "FK_LiveSessionMembers_LiveSessions_live_session_id",
                        column: x => x.live_session_id,
                        principalTable: "LiveSessions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_LiveSessionMembers_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LiveSessionModerators",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    live_session_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    updated_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiveSessionModerators", x => x.id);
                    table.ForeignKey(
                        name: "FK_LiveSessionModerators_LiveSessions_live_session_id",
                        column: x => x.live_session_id,
                        principalTable: "LiveSessions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_LiveSessionModerators_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LiveSessionReports",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    live_session_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    meeting_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    duration = table.Column<TimeSpan>(type: "time(6)", nullable: true),
                    join_time = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    left_time = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    start_time = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiveSessionReports", x => x.id);
                    table.ForeignKey(
                        name: "FK_LiveSessionReports_LiveSessions_live_session_id",
                        column: x => x.live_session_id,
                        principalTable: "LiveSessions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_LiveSessionReports_Meetings_meeting_id",
                        column: x => x.meeting_id,
                        principalTable: "Meetings",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_LiveSessionReports_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LiveSessionTags",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    live_session_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tag_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    updated_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiveSessionTags", x => x.id);
                    table.ForeignKey(
                        name: "FK_LiveSessionTags_LiveSessions_live_session_id",
                        column: x => x.live_session_id,
                        principalTable: "LiveSessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LiveSessionTags_Tags_tag_id",
                        column: x => x.tag_id,
                        principalTable: "Tags",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_LiveSessionTags_Users_created_by",
                        column: x => x.created_by,
                        principalTable: "Users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_live_session_id",
                table: "Meetings",
                column: "live_session_id");

            migrationBuilder.CreateIndex(
                name: "IX_LiveSessionMembers_live_session_id",
                table: "LiveSessionMembers",
                column: "live_session_id");

            migrationBuilder.CreateIndex(
                name: "IX_LiveSessionMembers_user_id",
                table: "LiveSessionMembers",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_LiveSessionModerators_live_session_id",
                table: "LiveSessionModerators",
                column: "live_session_id");

            migrationBuilder.CreateIndex(
                name: "IX_LiveSessionModerators_user_id",
                table: "LiveSessionModerators",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_LiveSessionReports_live_session_id",
                table: "LiveSessionReports",
                column: "live_session_id");

            migrationBuilder.CreateIndex(
                name: "IX_LiveSessionReports_meeting_id",
                table: "LiveSessionReports",
                column: "meeting_id");

            migrationBuilder.CreateIndex(
                name: "IX_LiveSessionReports_user_id",
                table: "LiveSessionReports",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_LiveSessions_created_by",
                table: "LiveSessions",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_LiveSessionTags_created_by",
                table: "LiveSessionTags",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_LiveSessionTags_live_session_id",
                table: "LiveSessionTags",
                column: "live_session_id");

            migrationBuilder.CreateIndex(
                name: "IX_LiveSessionTags_tag_id",
                table: "LiveSessionTags",
                column: "tag_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_LiveSessions_live_session_id",
                table: "Meetings",
                column: "live_session_id",
                principalTable: "LiveSessions",
                principalColumn: "id");
        }
    }
}

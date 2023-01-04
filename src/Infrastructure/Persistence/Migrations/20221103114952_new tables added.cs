using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class newtablesadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseTags",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tag_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    course_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<string>(type: "VARCHAR(50)", nullable: false)
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
                    table.PrimaryKey("PK_CourseTags", x => x.id);
                    table.ForeignKey(
                        name: "FK_CourseTags_Courses_course_id",
                        column: x => x.course_id,
                        principalTable: "Courses",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_CourseTags_Tags_tag_id",
                        column: x => x.tag_id,
                        principalTable: "Tags",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_CourseTags_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CourseTeachers",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    course_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
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
                    table.PrimaryKey("PK_CourseTeachers", x => x.id);
                    table.ForeignKey(
                        name: "FK_CourseTeachers_Courses_course_id",
                        column: x => x.course_id,
                        principalTable: "Courses",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_CourseTeachers_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LiveSessions",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    slug = table.Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ThumbnailUrl = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    start_date = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    end_date = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    event_type = table.Column<int>(type: "int", nullable: false),
                    recurrence = table.Column<string>(type: "VARCHAR(250)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<int>(type: "int", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    nearest_start_time = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    nearest_end_time = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    created_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
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
                name: "Sections",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    slug = table.Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    course_id = table.Column<string>(type: "VARCHAR(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    order = table.Column<int>(type: "int", nullable: false),
                    duration = table.Column<int>(type: "int", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    created_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    updated_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sections", x => x.id);
                    table.ForeignKey(
                        name: "FK_Sections_Courses_course_id",
                        column: x => x.course_id,
                        principalTable: "Courses",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Sections_Users_created_by",
                        column: x => x.created_by,
                        principalTable: "Users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SMTPSettings",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mail_server = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mail_port = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sender_email = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_name = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    replay_to = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    use_ssl = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    created_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    updated_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SMTPSettings", x => x.id);
                    table.ForeignKey(
                        name: "FK_SMTPSettings_Users_created_by",
                        column: x => x.created_by,
                        principalTable: "Users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ZoomLicenses",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    license_email = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    host_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
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
                    table.PrimaryKey("PK_ZoomLicenses", x => x.id);
                    table.ForeignKey(
                        name: "FK_ZoomLicenses_Users_created_by",
                        column: x => x.created_by,
                        principalTable: "Users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ZoomSettings",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    api_key = table.Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    secret_key = table.Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_recording_enabled = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    created_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    updated_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZoomSettings", x => x.id);
                    table.ForeignKey(
                        name: "FK_ZoomSettings_Users_created_by",
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
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    created_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
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
                name: "LiveSessionTags",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    live_session_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tag_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Meetings",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    meeting_number = table.Column<long>(type: "bigint", nullable: false),
                    PassCode = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    live_session_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
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
                    table.PrimaryKey("PK_Meetings", x => x.id);
                    table.ForeignKey(
                        name: "FK_Meetings_LiveSessions_live_session_id",
                        column: x => x.live_session_id,
                        principalTable: "LiveSessions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Meetings_Users_created_by",
                        column: x => x.created_by,
                        principalTable: "Users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Lessons",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    slug = table.Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    video_url = table.Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    thumbnail_url = table.Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    order = table.Column<int>(type: "int", nullable: false),
                    duration = table.Column<int>(type: "int", nullable: false),
                    is_preview = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    course_id = table.Column<string>(type: "VARCHAR(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    section_id = table.Column<string>(type: "VARCHAR(50)", nullable: false)
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
                    table.PrimaryKey("PK_Lessons", x => x.id);
                    table.ForeignKey(
                        name: "FK_Lessons_Courses_course_id",
                        column: x => x.course_id,
                        principalTable: "Courses",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Lessons_Sections_section_id",
                        column: x => x.section_id,
                        principalTable: "Sections",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Lessons_Users_created_by",
                        column: x => x.created_by,
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
                    meeting_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    live_session_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_CourseTags_course_id",
                table: "CourseTags",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTags_tag_id",
                table: "CourseTags",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTags_UserId",
                table: "CourseTags",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTeachers_course_id",
                table: "CourseTeachers",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTeachers_user_id",
                table: "CourseTeachers",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_course_id",
                table: "Lessons",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_created_by",
                table: "Lessons",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_section_id",
                table: "Lessons",
                column: "section_id");

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

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_created_by",
                table: "Meetings",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_live_session_id",
                table: "Meetings",
                column: "live_session_id");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_course_id",
                table: "Sections",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_created_by",
                table: "Sections",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_SMTPSettings_created_by",
                table: "SMTPSettings",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_ZoomLicenses_created_by",
                table: "ZoomLicenses",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_ZoomSettings_created_by",
                table: "ZoomSettings",
                column: "created_by");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseTags");

            migrationBuilder.DropTable(
                name: "CourseTeachers");

            migrationBuilder.DropTable(
                name: "Lessons");

            migrationBuilder.DropTable(
                name: "LiveSessionMembers");

            migrationBuilder.DropTable(
                name: "LiveSessionModerators");

            migrationBuilder.DropTable(
                name: "LiveSessionReports");

            migrationBuilder.DropTable(
                name: "LiveSessionTags");

            migrationBuilder.DropTable(
                name: "SMTPSettings");

            migrationBuilder.DropTable(
                name: "ZoomLicenses");

            migrationBuilder.DropTable(
                name: "ZoomSettings");

            migrationBuilder.DropTable(
                name: "Sections");

            migrationBuilder.DropTable(
                name: "Meetings");

            migrationBuilder.DropTable(
                name: "LiveSessions");
        }
    }
}

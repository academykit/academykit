﻿using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase().Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "Logs",
                    columns: table => new
                    {
                        Id = table
                            .Column<int>(type: "int", nullable: false)
                            .Annotation(
                                "MySql:ValueGenerationStrategy",
                                MySqlValueGenerationStrategy.IdentityColumn
                            ),
                        MachineName = table
                            .Column<string>(type: "VARCHAR(200)", nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        Level = table
                            .Column<string>(type: "VARCHAR(200)", maxLength: 200, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        Logged = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        Message = table
                            .Column<string>(type: "VARCHAR(4000)", nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        Logger = table
                            .Column<string>(type: "VARCHAR(400)", nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        Properties = table
                            .Column<string>(type: "VARCHAR(1000)", nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        Exception = table
                            .Column<string>(type: "VARCHAR(5000)", nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4")
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Logs", x => x.Id);
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "Settings",
                    columns: table => new
                    {
                        key = table
                            .Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        value = table
                            .Column<string>(type: "VARCHAR(200)", maxLength: 200, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4")
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Settings", x => x.key);
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "AssignmentAttachments",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        assignment_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        file_url = table
                            .Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        order = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                        name = table
                            .Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        mime_type = table
                            .Column<string>(type: "VARCHAR(50)", nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_AssignmentAttachments", x => x.id);
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "AssignmentQuestionOptions",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        assignment_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        option = table
                            .Column<string>(type: "VARCHAR(5000)", maxLength: 5000, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        is_correct = table.Column<bool>(
                            type: "tinyint(1)",
                            nullable: false,
                            defaultValue: false
                        ),
                        order = table.Column<int>(type: "int", nullable: false),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_AssignmentQuestionOptions", x => x.id);
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "AssignmentReviews",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        lesson_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        user_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        mark = table.Column<decimal>(type: "decimal(20,4)", nullable: false),
                        review = table
                            .Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        is_deleted = table.Column<bool>(
                            type: "tinyint(1)",
                            nullable: false,
                            defaultValue: false
                        ),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_AssignmentReviews", x => x.id);
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "Assignments",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        lesson_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        name = table
                            .Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        description = table
                            .Column<string>(type: "VARCHAR(5000)", maxLength: 5000, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        hints = table
                            .Column<string>(type: "VARCHAR(5000)", maxLength: 5000, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        order = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                        is_active = table.Column<bool>(
                            type: "tinyint(1)",
                            nullable: false,
                            defaultValue: false
                        ),
                        type = table.Column<int>(type: "int", nullable: false),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Assignments", x => x.id);
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "AssignmentSubmissionAttachments",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        assignment_submission_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        name = table
                            .Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        mime_type = table
                            .Column<string>(type: "VARCHAR(50)", nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        file_url = table
                            .Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_AssignmentSubmissionAttachments", x => x.id);
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "AssignmentSubmissions",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        lesson_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        assignment_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        user_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        is_correct = table.Column<bool>(
                            type: "tinyint(1)",
                            nullable: false,
                            defaultValue: false
                        ),
                        selected_option = table
                            .Column<string>(type: "VARCHAR(300)", maxLength: 300, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        answer = table
                            .Column<string>(type: "VARCHAR(5000)", maxLength: 5000, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_AssignmentSubmissions", x => x.id);
                        table.ForeignKey(
                            name: "FK_AssignmentSubmissions_Assignments_assignment_id",
                            column: x => x.assignment_id,
                            principalTable: "Assignments",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "Certificates",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        name = table
                            .Column<string>(type: "VARCHAR(200)", maxLength: 200, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        start_date = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        end_date = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        image_url = table
                            .Column<string>(type: "VARCHAR(200)", nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        location = table
                            .Column<string>(type: "VARCHAR(100)", nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        institute = table
                            .Column<string>(type: "VARCHAR(100)", nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        duration = table.Column<int>(type: "int", nullable: true),
                        status = table.Column<int>(type: "int", nullable: false),
                        optional_cost = table.Column<decimal>(
                            type: "DECIMAL(10,2)",
                            nullable: false,
                            defaultValue: 0m
                        ),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Certificates", x => x.id);
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "CommentReplies",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        comment_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        content = table
                            .Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        is_deleted = table.Column<bool>(
                            type: "tinyint(1)",
                            nullable: false,
                            defaultValue: false
                        ),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_CommentReplies", x => x.id);
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "Comments",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        course_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        content = table
                            .Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        is_deleted = table.Column<bool>(
                            type: "tinyint(1)",
                            nullable: false,
                            defaultValue: false
                        ),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Comments", x => x.id);
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "CourseCertificate",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        title = table
                            .Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        course_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        event_start_date = table.Column<DateTime>(
                            type: "DATETIME",
                            nullable: false
                        ),
                        event_end_date = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        sample_url = table
                            .Column<string>(type: "VARCHAR(500)", nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_CourseCertificate", x => x.id);
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "CourseEnrollments",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        current_lesson_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        current_lesson_watched = table.Column<int>(type: "int", nullable: false),
                        percentage = table.Column<int>(
                            type: "int",
                            nullable: false,
                            defaultValue: 0
                        ),
                        status = table.Column<int>(type: "int", nullable: false),
                        activity_reason = table
                            .Column<string>(type: "VARCHAR(1000)", maxLength: 1000, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        course_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        user_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        enrollment_date = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        is_deleted = table.Column<ulong>(
                            type: "BIT",
                            nullable: false,
                            defaultValue: 0ul
                        ),
                        deleted_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        deleted_on = table.Column<DateTime>(type: "DATETIME", nullable: true),
                        has_certificate_issued = table.Column<bool>(
                            type: "tinyint(1)",
                            nullable: true
                        ),
                        certificate_url = table
                            .Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        certificate_issued_date = table.Column<DateTime>(
                            type: "DATETIME",
                            nullable: true
                        ),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_CourseEnrollments", x => x.id);
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "Courses",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        slug = table
                            .Column<string>(type: "VARCHAR(270)", maxLength: 270, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        name = table
                            .Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        description = table
                            .Column<string>(type: "VARCHAR(5000)", maxLength: 5000, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        group_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        status = table.Column<int>(type: "int", nullable: false),
                        language = table.Column<int>(type: "int", nullable: false),
                        thumbnail_url = table
                            .Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        duration = table.Column<int>(type: "int", nullable: false),
                        is_update = table.Column<bool>(
                            type: "tinyint(1)",
                            nullable: false,
                            defaultValue: false
                        ),
                        level_id = table
                            .Column<string>(type: "VARCHAR(50)", nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Courses", x => x.id);
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "CourseTags",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        tag_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        course_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
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
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "CourseTeachers",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        user_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        course_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
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
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "Departments",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        slug = table
                            .Column<string>(type: "VARCHAR(270)", maxLength: 270, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        name = table
                            .Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        is_active = table.Column<bool>(
                            type: "tinyint(1)",
                            nullable: false,
                            defaultValue: false
                        ),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Departments", x => x.id);
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "Users",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        member_id = table
                            .Column<string>(type: "VARCHAR(50)", nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        first_name = table
                            .Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        middle_name = table
                            .Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        last_name = table
                            .Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        email = table
                            .Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        mobile_number = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        role = table.Column<int>(type: "int", nullable: false, defaultValue: 4),
                        profession = table
                            .Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        address = table
                            .Column<string>(type: "VARCHAR(200)", maxLength: 200, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        bio = table
                            .Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        hash_password = table
                            .Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        public_urls = table
                            .Column<string>(type: "VARCHAR(2000)", maxLength: 2000, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        status = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                        password_reset_token = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        image_url = table
                            .Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        password_reset_token_expiry = table.Column<DateTime>(
                            type: "DATETIME",
                            nullable: true
                        ),
                        password_change_token = table
                            .Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        department_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Users", x => x.id);
                        table.ForeignKey(
                            name: "FK_Users_Departments_department_id",
                            column: x => x.department_id,
                            principalTable: "Departments",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "GeneralSettings",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        logo_url = table
                            .Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        company_name = table
                            .Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        company_address = table
                            .Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        company_contact_number = table
                            .Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        email_signature = table
                            .Column<string>(type: "VARCHAR(1000)", maxLength: 1000, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        custom_configuration = table
                            .Column<string>(type: "VARCHAR(5000)", nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_GeneralSettings", x => x.id);
                        table.ForeignKey(
                            name: "FK_GeneralSettings_Users_created_by",
                            column: x => x.created_by,
                            principalTable: "Users",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "Groups",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        slug = table
                            .Column<string>(type: "VARCHAR(270)", maxLength: 270, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        name = table
                            .Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        is_active = table.Column<bool>(
                            type: "tinyint(1)",
                            nullable: false,
                            defaultValue: false
                        ),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Groups", x => x.id);
                        table.ForeignKey(
                            name: "FK_Groups_Users_created_by",
                            column: x => x.created_by,
                            principalTable: "Users",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "Levels",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        slug = table
                            .Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        name = table
                            .Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        is_active = table.Column<bool>(
                            type: "tinyint(1)",
                            nullable: false,
                            defaultValue: false
                        ),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Levels", x => x.id);
                        table.ForeignKey(
                            name: "FK_Levels_Users_created_by",
                            column: x => x.created_by,
                            principalTable: "Users",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "QuestionPools",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        name = table
                            .Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        slug = table
                            .Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        is_deleted = table.Column<bool>(
                            type: "tinyint(1)",
                            nullable: false,
                            defaultValue: false
                        ),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_QuestionPools", x => x.id);
                        table.ForeignKey(
                            name: "FK_QuestionPools_Users_created_by",
                            column: x => x.created_by,
                            principalTable: "Users",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "Questions",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        name = table
                            .Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        type = table.Column<int>(type: "int", nullable: false, defaultValue: 2),
                        description = table
                            .Column<string>(type: "varchar(5000)", maxLength: 5000, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        hints = table
                            .Column<string>(type: "varchar(5000)", maxLength: 5000, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Questions", x => x.id);
                        table.ForeignKey(
                            name: "FK_Questions_Users_created_by",
                            column: x => x.created_by,
                            principalTable: "Users",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "QuestionSets",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        name = table
                            .Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        slug = table
                            .Column<string>(type: "VARCHAR(520)", maxLength: 520, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        thumbnail_url = table
                            .Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        description = table
                            .Column<string>(type: "varchar(5000)", maxLength: 5000, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        negative_marking = table.Column<decimal>(
                            type: "decimal(10,4)",
                            nullable: false,
                            defaultValue: 0m
                        ),
                        question_marking = table.Column<decimal>(
                            type: "decimal(10,4)",
                            nullable: false
                        ),
                        passing_weightage = table.Column<decimal>(
                            type: "decimal(10,4)",
                            nullable: false
                        ),
                        allowed_retake = table.Column<int>(
                            type: "int",
                            nullable: false,
                            defaultValue: 1
                        ),
                        duration = table.Column<int>(type: "int", nullable: false),
                        start_time = table.Column<DateTime>(type: "DATETIME", nullable: true),
                        end_time = table.Column<DateTime>(type: "DATETIME", nullable: true),
                        is_deleted = table.Column<bool>(
                            type: "tinyint(1)",
                            nullable: false,
                            defaultValue: false
                        ),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_QuestionSets", x => x.id);
                        table.ForeignKey(
                            name: "FK_QuestionSets_Users_created_by",
                            column: x => x.created_by,
                            principalTable: "Users",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "RefreshTokens",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        user_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        token = table
                            .Column<string>(type: "VARCHAR(200)", maxLength: 200, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        login_at = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        device_id = table
                            .Column<string>(type: "VARCHAR(200)", maxLength: 200, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        location = table
                            .Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        is_active = table.Column<bool>(
                            type: "tinyint(1)",
                            nullable: false,
                            defaultValue: false
                        ),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_RefreshTokens", x => x.id);
                        table.ForeignKey(
                            name: "FK_RefreshTokens_Users_user_id",
                            column: x => x.user_id,
                            principalTable: "Users",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "Sections",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        slug = table
                            .Column<string>(type: "VARCHAR(270)", maxLength: 270, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        name = table
                            .Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        description = table
                            .Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        course_id = table
                            .Column<string>(type: "VARCHAR(50)", nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        order = table.Column<int>(type: "int", nullable: false),
                        duration = table.Column<int>(type: "int", nullable: false),
                        is_deleted = table.Column<bool>(
                            type: "tinyint(1)",
                            nullable: false,
                            defaultValue: false
                        ),
                        status = table.Column<int>(type: "int", nullable: false),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
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
                            principalColumn: "id"
                        );
                        table.ForeignKey(
                            name: "FK_Sections_Users_created_by",
                            column: x => x.created_by,
                            principalTable: "Users",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "Signature",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        course_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        full_name = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        designation = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        file_url = table
                            .Column<string>(type: "VARCHAR(200)", maxLength: 200, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Signature", x => x.id);
                        table.ForeignKey(
                            name: "FK_Signature_Courses_course_id",
                            column: x => x.course_id,
                            principalTable: "Courses",
                            principalColumn: "id",
                            onDelete: ReferentialAction.Cascade
                        );
                        table.ForeignKey(
                            name: "FK_Signature_Users_created_by",
                            column: x => x.created_by,
                            principalTable: "Users",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "SMTPSettings",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        mail_server = table
                            .Column<string>(type: "VARCHAR(200)", maxLength: 200, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        mail_port = table.Column<int>(type: "int", nullable: false),
                        sender_name = table
                            .Column<string>(type: "VARCHAR(200)", maxLength: 200, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        sender_email = table
                            .Column<string>(type: "VARCHAR(200)", maxLength: 200, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        user_name = table
                            .Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        password = table
                            .Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        reply_to = table
                            .Column<string>(type: "VARCHAR(200)", maxLength: 200, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        UseSSL = table.Column<bool>(type: "tinyint(1)", nullable: false),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
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
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "Tags",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        slug = table
                            .Column<string>(type: "VARCHAR(270)", maxLength: 270, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        name = table
                            .Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        is_active = table.Column<bool>(
                            type: "tinyint(1)",
                            nullable: false,
                            defaultValue: false
                        ),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Tags", x => x.id);
                        table.ForeignKey(
                            name: "FK_Tags_Users_created_by",
                            column: x => x.created_by,
                            principalTable: "Users",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "ZoomLicenses",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        license_email = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        host_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        capacity = table.Column<int>(type: "int", nullable: false),
                        is_active = table.Column<bool>(
                            type: "tinyint(1)",
                            nullable: false,
                            defaultValue: true
                        ),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
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
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "ZoomSettings",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        sdk_key = table
                            .Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        sdk_secret = table
                            .Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        webhook_secret = table
                            .Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        oauth_account_id = table
                            .Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        oauth_client_id = table
                            .Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        oauth_client_secret = table
                            .Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        is_recording_enabled = table.Column<bool>(
                            type: "tinyint(1)",
                            nullable: false,
                            defaultValue: false
                        ),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
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
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "GroupFiles",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        GroupId = table
                            .Column<string>(type: "VARCHAR(50)", nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        name = table
                            .Column<string>(type: "VARCHAR(200)", maxLength: 200, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        Size = table.Column<double>(type: "double", nullable: false),
                        url = table
                            .Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        mime_type = table
                            .Column<string>(type: "VARCHAR(200)", maxLength: 200, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_GroupFiles", x => x.id);
                        table.ForeignKey(
                            name: "FK_GroupFiles_Groups_GroupId",
                            column: x => x.GroupId,
                            principalTable: "Groups",
                            principalColumn: "id"
                        );
                        table.ForeignKey(
                            name: "FK_GroupFiles_Users_created_by",
                            column: x => x.created_by,
                            principalTable: "Users",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "GroupMembers",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        user_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        group_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        is_active = table.Column<bool>(
                            type: "tinyint(1)",
                            nullable: false,
                            defaultValue: false
                        ),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_GroupMembers", x => x.id);
                        table.ForeignKey(
                            name: "FK_GroupMembers_Groups_group_id",
                            column: x => x.group_id,
                            principalTable: "Groups",
                            principalColumn: "id",
                            onDelete: ReferentialAction.Cascade
                        );
                        table.ForeignKey(
                            name: "FK_GroupMembers_Users_user_id",
                            column: x => x.user_id,
                            principalTable: "Users",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "QuestionPoolTeachers",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        user_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        question_pool_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        role = table.Column<int>(type: "int", nullable: false, defaultValue: 2),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_QuestionPoolTeachers", x => x.id);
                        table.ForeignKey(
                            name: "FK_QuestionPoolTeachers_QuestionPools_question_pool_id",
                            column: x => x.question_pool_id,
                            principalTable: "QuestionPools",
                            principalColumn: "id",
                            onDelete: ReferentialAction.Cascade
                        );
                        table.ForeignKey(
                            name: "FK_QuestionPoolTeachers_Users_user_id",
                            column: x => x.user_id,
                            principalTable: "Users",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "QuestionOptions",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        option = table
                            .Column<string>(type: "varchar(5000)", maxLength: 5000, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        order = table.Column<int>(type: "int", nullable: false),
                        question_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        is_correct = table.Column<bool>(
                            type: "tinyint(1)",
                            nullable: false,
                            defaultValue: false
                        ),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_QuestionOptions", x => x.id);
                        table.ForeignKey(
                            name: "FK_QuestionOptions_Questions_question_id",
                            column: x => x.question_id,
                            principalTable: "Questions",
                            principalColumn: "id"
                        );
                        table.ForeignKey(
                            name: "FK_QuestionOptions_Users_created_by",
                            column: x => x.created_by,
                            principalTable: "Users",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "QuestionPoolQuestions",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        question_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        question_pool_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        order = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_QuestionPoolQuestions", x => x.id);
                        table.ForeignKey(
                            name: "FK_QuestionPoolQuestions_QuestionPools_question_pool_id",
                            column: x => x.question_pool_id,
                            principalTable: "QuestionPools",
                            principalColumn: "id"
                        );
                        table.ForeignKey(
                            name: "FK_QuestionPoolQuestions_Questions_question_id",
                            column: x => x.question_id,
                            principalTable: "Questions",
                            principalColumn: "id"
                        );
                        table.ForeignKey(
                            name: "FK_QuestionPoolQuestions_Users_created_by",
                            column: x => x.created_by,
                            principalTable: "Users",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "QuestionSetSubmissions",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        question_set_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        user_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        start_time = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        end_time = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        is_submission_error = table.Column<bool>(
                            type: "tinyint(1)",
                            nullable: false,
                            defaultValue: false
                        ),
                        submission_error_message = table
                            .Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_QuestionSetSubmissions", x => x.id);
                        table.ForeignKey(
                            name: "FK_QuestionSetSubmissions_QuestionSets_question_set_id",
                            column: x => x.question_set_id,
                            principalTable: "QuestionSets",
                            principalColumn: "id"
                        );
                        table.ForeignKey(
                            name: "FK_QuestionSetSubmissions_Users_user_id",
                            column: x => x.user_id,
                            principalTable: "Users",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "QuestionTags",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        tag_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        question_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_QuestionTags", x => x.id);
                        table.ForeignKey(
                            name: "FK_QuestionTags_Questions_question_id",
                            column: x => x.question_id,
                            principalTable: "Questions",
                            principalColumn: "id",
                            onDelete: ReferentialAction.Cascade
                        );
                        table.ForeignKey(
                            name: "FK_QuestionTags_Tags_tag_id",
                            column: x => x.tag_id,
                            principalTable: "Tags",
                            principalColumn: "id",
                            onDelete: ReferentialAction.Cascade
                        );
                        table.ForeignKey(
                            name: "FK_QuestionTags_Users_created_by",
                            column: x => x.created_by,
                            principalTable: "Users",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "Meetings",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        meeting_number = table.Column<long>(type: "bigint", nullable: true),
                        passcode = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        zoom_license_id = table
                            .Column<string>(type: "VARCHAR(50)", nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        duration = table.Column<int>(type: "int", nullable: false),
                        start_date = table.Column<DateTime>(type: "DATETIME", nullable: true),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Meetings", x => x.id);
                        table.ForeignKey(
                            name: "FK_Meetings_Users_created_by",
                            column: x => x.created_by,
                            principalTable: "Users",
                            principalColumn: "id"
                        );
                        table.ForeignKey(
                            name: "FK_Meetings_ZoomLicenses_zoom_license_id",
                            column: x => x.zoom_license_id,
                            principalTable: "ZoomLicenses",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "QuestionSetQuestions",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        question_set_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        question_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        order = table.Column<int>(type: "int", nullable: false),
                        question_pool_question_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_QuestionSetQuestions", x => x.id);
                        table.ForeignKey(
                            name: "FK_QuestionSetQuestions_QuestionPoolQuestions_question_pool_que~",
                            column: x => x.question_pool_question_id,
                            principalTable: "QuestionPoolQuestions",
                            principalColumn: "id"
                        );
                        table.ForeignKey(
                            name: "FK_QuestionSetQuestions_QuestionSets_question_set_id",
                            column: x => x.question_set_id,
                            principalTable: "QuestionSets",
                            principalColumn: "id"
                        );
                        table.ForeignKey(
                            name: "FK_QuestionSetQuestions_Questions_question_id",
                            column: x => x.question_id,
                            principalTable: "Questions",
                            principalColumn: "id"
                        );
                        table.ForeignKey(
                            name: "FK_QuestionSetQuestions_Users_created_by",
                            column: x => x.created_by,
                            principalTable: "Users",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "QuestionSetResults",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        user_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        question_set_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        question_set_submission_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        total_mark = table.Column<decimal>(type: "decimal(20,4)", nullable: false),
                        negative_mark = table.Column<decimal>(
                            type: "decimal(20,4)",
                            nullable: false
                        ),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_QuestionSetResults", x => x.id);
                        table.ForeignKey(
                            name: "FK_QuestionSetResults_QuestionSetSubmissions_question_set_submi~",
                            column: x => x.question_set_submission_id,
                            principalTable: "QuestionSetSubmissions",
                            principalColumn: "id"
                        );
                        table.ForeignKey(
                            name: "FK_QuestionSetResults_QuestionSets_question_set_id",
                            column: x => x.question_set_id,
                            principalTable: "QuestionSets",
                            principalColumn: "id"
                        );
                        table.ForeignKey(
                            name: "FK_QuestionSetResults_Users_user_id",
                            column: x => x.user_id,
                            principalTable: "Users",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "Lessons",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        slug = table
                            .Column<string>(type: "VARCHAR(270)", maxLength: 270, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        name = table
                            .Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        description = table
                            .Column<string>(type: "VARCHAR(5000)", maxLength: 5000, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        video_url = table
                            .Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        thumbnail_url = table
                            .Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        document_url = table
                            .Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        order = table.Column<int>(type: "int", nullable: false),
                        duration = table.Column<int>(type: "int", nullable: false),
                        is_mandatory = table.Column<bool>(
                            type: "tinyint(1)",
                            nullable: false,
                            defaultValue: false
                        ),
                        type = table.Column<int>(type: "int", nullable: false),
                        is_deleted = table.Column<bool>(
                            type: "tinyint(1)",
                            nullable: false,
                            defaultValue: false
                        ),
                        status = table.Column<int>(type: "int", nullable: false),
                        course_id = table
                            .Column<string>(type: "VARCHAR(50)", nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        section_id = table
                            .Column<string>(type: "VARCHAR(50)", nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        meeting_id = table
                            .Column<string>(type: "VARCHAR(50)", nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        question_set_id = table
                            .Column<string>(type: "VARCHAR(50)", nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        start_date = table.Column<DateTime>(type: "DATETIME", nullable: true),
                        end_date = table.Column<DateTime>(type: "DATETIME", nullable: true),
                        video_key = table
                            .Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
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
                            principalColumn: "id"
                        );
                        table.ForeignKey(
                            name: "FK_Lessons_Meetings_meeting_id",
                            column: x => x.meeting_id,
                            principalTable: "Meetings",
                            principalColumn: "id"
                        );
                        table.ForeignKey(
                            name: "FK_Lessons_QuestionSets_question_set_id",
                            column: x => x.question_set_id,
                            principalTable: "QuestionSets",
                            principalColumn: "id"
                        );
                        table.ForeignKey(
                            name: "FK_Lessons_Sections_section_id",
                            column: x => x.section_id,
                            principalTable: "Sections",
                            principalColumn: "id"
                        );
                        table.ForeignKey(
                            name: "FK_Lessons_Users_created_by",
                            column: x => x.created_by,
                            principalTable: "Users",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "MeetingReports",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        meeting_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        user_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
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
                            principalColumn: "id"
                        );
                        table.ForeignKey(
                            name: "FK_MeetingReports_Users_user_id",
                            column: x => x.user_id,
                            principalTable: "Users",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "QuestionSetSubmissionAnswers",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        question_set_submission_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        question_set_question_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        selected_answers = table
                            .Column<string>(type: "VARCHAR(150)", maxLength: 150, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        is_correct = table.Column<bool>(
                            type: "tinyint(1)",
                            nullable: false,
                            defaultValue: false
                        ),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_QuestionSetSubmissionAnswers", x => x.id);
                        table.ForeignKey(
                            name: "FK_QuestionSetSubmissionAnswers_QuestionSetQuestions_question_s~",
                            column: x => x.question_set_question_id,
                            principalTable: "QuestionSetQuestions",
                            principalColumn: "id",
                            onDelete: ReferentialAction.Cascade
                        );
                        table.ForeignKey(
                            name: "FK_QuestionSetSubmissionAnswers_QuestionSetSubmissions_question~",
                            column: x => x.question_set_submission_id,
                            principalTable: "QuestionSetSubmissions",
                            principalColumn: "id"
                        );
                        table.ForeignKey(
                            name: "FK_QuestionSetSubmissionAnswers_Users_created_by",
                            column: x => x.created_by,
                            principalTable: "Users",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "Feedbacks",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        lesson_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        name = table
                            .Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        order = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                        is_active = table.Column<bool>(
                            type: "tinyint(1)",
                            nullable: false,
                            defaultValue: false
                        ),
                        type = table.Column<int>(type: "int", nullable: false),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Feedbacks", x => x.id);
                        table.ForeignKey(
                            name: "FK_Feedbacks_Lessons_lesson_id",
                            column: x => x.lesson_id,
                            principalTable: "Lessons",
                            principalColumn: "id",
                            onDelete: ReferentialAction.Cascade
                        );
                        table.ForeignKey(
                            name: "FK_Feedbacks_Users_created_by",
                            column: x => x.created_by,
                            principalTable: "Users",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "PhysicalLessonReviews",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        lesson_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        review_message = table
                            .Column<string>(type: "VARCHAR(500)", nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        user_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        hasAttended = table.Column<bool>(
                            type: "tinyint(1)",
                            nullable: false,
                            defaultValue: false
                        ),
                        is_reviewed = table.Column<bool>(
                            type: "tinyint(1)",
                            nullable: false,
                            defaultValue: false
                        ),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_PhysicalLessonReviews", x => x.id);
                        table.ForeignKey(
                            name: "FK_PhysicalLessonReviews_Lessons_lesson_id",
                            column: x => x.lesson_id,
                            principalTable: "Lessons",
                            principalColumn: "id"
                        );
                        table.ForeignKey(
                            name: "FK_PhysicalLessonReviews_Users_user_id",
                            column: x => x.user_id,
                            principalTable: "Users",
                            principalColumn: "id",
                            onDelete: ReferentialAction.Cascade
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "VideoQueue",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        lesson_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        video_url = table
                            .Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: false)
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
                            onDelete: ReferentialAction.Cascade
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "WatchHistories",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        course_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        lesson_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        user_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        is_completed = table.Column<bool>(
                            type: "tinyint(1)",
                            nullable: false,
                            defaultValue: false
                        ),
                        is_passed = table.Column<bool>(
                            type: "tinyint(1)",
                            nullable: false,
                            defaultValue: false
                        ),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_WatchHistories", x => x.id);
                        table.ForeignKey(
                            name: "FK_WatchHistories_Courses_course_id",
                            column: x => x.course_id,
                            principalTable: "Courses",
                            principalColumn: "id",
                            onDelete: ReferentialAction.Cascade
                        );
                        table.ForeignKey(
                            name: "FK_WatchHistories_Lessons_lesson_id",
                            column: x => x.lesson_id,
                            principalTable: "Lessons",
                            principalColumn: "id"
                        );
                        table.ForeignKey(
                            name: "FK_WatchHistories_Users_user_id",
                            column: x => x.user_id,
                            principalTable: "Users",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "FeedbackQuestionOptions",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        feedback_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        option = table
                            .Column<string>(type: "VARCHAR(5000)", maxLength: 5000, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        order = table.Column<int>(type: "int", nullable: false),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_FeedbackQuestionOptions", x => x.id);
                        table.ForeignKey(
                            name: "FK_FeedbackQuestionOptions_Feedbacks_feedback_id",
                            column: x => x.feedback_id,
                            principalTable: "Feedbacks",
                            principalColumn: "id"
                        );
                        table.ForeignKey(
                            name: "FK_FeedbackQuestionOptions_Users_created_by",
                            column: x => x.created_by,
                            principalTable: "Users",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "FeedbackSubmissions",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        lesson_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        feedback_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        user_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        selected_option = table
                            .Column<string>(type: "VARCHAR(300)", maxLength: 300, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        answer = table
                            .Column<string>(type: "VARCHAR(5000)", maxLength: 5000, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        rating = table.Column<int>(type: "int", nullable: true),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_FeedbackSubmissions", x => x.id);
                        table.ForeignKey(
                            name: "FK_FeedbackSubmissions_Feedbacks_feedback_id",
                            column: x => x.feedback_id,
                            principalTable: "Feedbacks",
                            principalColumn: "id"
                        );
                        table.ForeignKey(
                            name: "FK_FeedbackSubmissions_Lessons_lesson_id",
                            column: x => x.lesson_id,
                            principalTable: "Lessons",
                            principalColumn: "id",
                            onDelete: ReferentialAction.Cascade
                        );
                        table.ForeignKey(
                            name: "FK_FeedbackSubmissions_Users_user_id",
                            column: x => x.user_id,
                            principalTable: "Users",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "key", "value" },
                values: new object[,]
                {
                    { "AWS_AccessKey", null },
                    { "AWS_CloudFront", null },
                    { "AWS_FileBucket", null },
                    { "AWS_RegionEndpoint", null },
                    { "AWS_SecretKey", null },
                    { "AWS_VideoBucket", null },
                    { "Server_AccessKey", null },
                    { "Server_Bucket", null },
                    { "Server_EndPoint", null },
                    { "Server_PresignedExpiryTime", null },
                    { "Server_PresignedUrl", null },
                    { "Server_SecretKey", null },
                    { "Server_Url", null },
                    { "Storage", "AWS" }
                }
            );

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[]
                {
                    "id",
                    "address",
                    "bio",
                    "created_by",
                    "created_on",
                    "department_id",
                    "email",
                    "first_name",
                    "hash_password",
                    "image_url",
                    "last_name",
                    "member_id",
                    "middle_name",
                    "mobile_number",
                    "password_change_token",
                    "password_reset_token",
                    "password_reset_token_expiry",
                    "profession",
                    "public_urls",
                    "role",
                    "status",
                    "updated_by",
                    "updated_on"
                },
                values: new object[]
                {
                    "30fcd978-f256-4733-840f-759181bc5e63",
                    "ADDRESS",
                    null,
                    "30fcd978-f256-4733-840f-759181bc5e63",
                    new DateTime(2022, 11, 4, 10, 35, 19, 307, DateTimeKind.Utc).AddTicks(3004),
                    null,
                    "academykit@yopmail.com",
                    "Superadmin",
                    "SKN7zJDnqOtL0+4zVwx68H3p12LoJw0kNZ06d1wRs28=:pbOoXC3dmcTXhib+l/nYuQ==",
                    null,
                    "User",
                    null,
                    null,
                    "1234567890",
                    null,
                    null,
                    null,
                    null,
                    null,
                    1,
                    1,
                    "30fcd978-f256-4733-840f-759181bc5e63",
                    new DateTime(2022, 11, 4, 10, 35, 19, 307, DateTimeKind.Utc).AddTicks(3004)
                }
            );

            migrationBuilder.InsertData(
                table: "GeneralSettings",
                columns: new[]
                {
                    "id",
                    "company_address",
                    "company_contact_number",
                    "company_name",
                    "created_by",
                    "created_on",
                    "custom_configuration",
                    "email_signature",
                    "logo_url",
                    "updated_by",
                    "updated_on"
                },
                values: new object[]
                {
                    "2d7867fc-b7e7-461d-9257-d0990b5ac991",
                    "company address",
                    "company contact number",
                    "company name",
                    "30fcd978-f256-4733-840f-759181bc5e63",
                    new DateTime(2022, 11, 4, 10, 35, 19, 307, DateTimeKind.Utc).AddTicks(3004),
                    null,
                    "company default email signature",
                    "image path",
                    "30fcd978-f256-4733-840f-759181bc5e63",
                    new DateTime(2022, 11, 4, 10, 35, 19, 307, DateTimeKind.Utc).AddTicks(3004)
                }
            );

            migrationBuilder.InsertData(
                table: "Levels",
                columns: new[]
                {
                    "id",
                    "created_by",
                    "created_on",
                    "name",
                    "slug",
                    "updated_by",
                    "updated_on"
                },
                values: new object[,]
                {
                    {
                        "7df8d749-6172-482b-b5a1-016fbe478795",
                        "30fcd978-f256-4733-840f-759181bc5e63",
                        new DateTime(2022, 11, 4, 10, 35, 19, 307, DateTimeKind.Utc).AddTicks(3004),
                        "Intermediate",
                        "intermediate",
                        null,
                        null
                    },
                    {
                        "7e6ff101-cfa2-4aec-bd25-42780be476c3",
                        "30fcd978-f256-4733-840f-759181bc5e63",
                        new DateTime(2022, 11, 4, 10, 35, 19, 307, DateTimeKind.Utc).AddTicks(3004),
                        "Beginner",
                        "beginner",
                        null,
                        null
                    },
                    {
                        "9be84cd8-1566-4af5-8442-61cb1796dc46",
                        "30fcd978-f256-4733-840f-759181bc5e63",
                        new DateTime(2022, 11, 4, 10, 35, 19, 307, DateTimeKind.Utc).AddTicks(3004),
                        "Advanced",
                        "advanced",
                        null,
                        null
                    }
                }
            );

            migrationBuilder.InsertData(
                table: "SMTPSettings",
                columns: new[]
                {
                    "id",
                    "created_by",
                    "created_on",
                    "mail_port",
                    "mail_server",
                    "password",
                    "reply_to",
                    "sender_email",
                    "sender_name",
                    "updated_by",
                    "updated_on",
                    "UseSSL",
                    "user_name"
                },
                values: new object[]
                {
                    "d3c343d8-adf8-45d4-afbe-e09c3285da24",
                    "30fcd978-f256-4733-840f-759181bc5e63",
                    new DateTime(2022, 11, 4, 10, 35, 19, 307, DateTimeKind.Utc).AddTicks(3004),
                    123,
                    "email-smtp.ap-south-1.amazonaws.com",
                    "password",
                    "hello@academykit.co",
                    "noreply@academykit.co",
                    "AcademyKit",
                    "30fcd978-f256-4733-840f-759181bc5e63",
                    new DateTime(2022, 11, 4, 10, 35, 19, 307, DateTimeKind.Utc).AddTicks(3004),
                    true,
                    "username"
                }
            );

            migrationBuilder.InsertData(
                table: "ZoomSettings",
                columns: new[]
                {
                    "id",
                    "created_by",
                    "created_on",
                    "oauth_account_id",
                    "oauth_client_id",
                    "oauth_client_secret",
                    "sdk_key",
                    "sdk_secret",
                    "updated_by",
                    "updated_on",
                    "webhook_secret"
                },
                values: new object[]
                {
                    "f41a902f-fabd-4749-ac28-91137f685cb8",
                    "30fcd978-f256-4733-840f-759181bc5e63",
                    new DateTime(2022, 11, 4, 10, 35, 19, 307, DateTimeKind.Utc).AddTicks(3004),
                    "OAuth account id",
                    "OAuth client id",
                    "OAuth client secret",
                    "sdk key value",
                    "sdk secret value",
                    "30fcd978-f256-4733-840f-759181bc5e63",
                    new DateTime(2022, 11, 4, 10, 35, 19, 307, DateTimeKind.Utc).AddTicks(3004),
                    null
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentAttachments_assignment_id",
                table: "AssignmentAttachments",
                column: "assignment_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentAttachments_created_by",
                table: "AssignmentAttachments",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentQuestionOptions_assignment_id",
                table: "AssignmentQuestionOptions",
                column: "assignment_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentQuestionOptions_created_by",
                table: "AssignmentQuestionOptions",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentReviews_lesson_id",
                table: "AssignmentReviews",
                column: "lesson_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentReviews_user_id",
                table: "AssignmentReviews",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_created_by",
                table: "Assignments",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_lesson_id",
                table: "Assignments",
                column: "lesson_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentSubmissionAttachments_assignment_submission_id",
                table: "AssignmentSubmissionAttachments",
                column: "assignment_submission_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentSubmissionAttachments_created_by",
                table: "AssignmentSubmissionAttachments",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentSubmissions_assignment_id",
                table: "AssignmentSubmissions",
                column: "assignment_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentSubmissions_lesson_id",
                table: "AssignmentSubmissions",
                column: "lesson_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentSubmissions_user_id",
                table: "AssignmentSubmissions",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_created_by",
                table: "Certificates",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_CommentReplies_comment_id",
                table: "CommentReplies",
                column: "comment_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_CommentReplies_created_by",
                table: "CommentReplies",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Comments_course_id",
                table: "Comments",
                column: "course_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Comments_created_by",
                table: "Comments",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_CourseCertificate_course_id",
                table: "CourseCertificate",
                column: "course_id",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_CourseCertificate_created_by",
                table: "CourseCertificate",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollments_course_id",
                table: "CourseEnrollments",
                column: "course_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollments_current_lesson_id",
                table: "CourseEnrollments",
                column: "current_lesson_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollments_user_id",
                table: "CourseEnrollments",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Courses_created_by",
                table: "Courses",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Courses_group_id",
                table: "Courses",
                column: "group_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Courses_level_id",
                table: "Courses",
                column: "level_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_CourseTags_course_id",
                table: "CourseTags",
                column: "course_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_CourseTags_created_by",
                table: "CourseTags",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_CourseTags_tag_id",
                table: "CourseTags",
                column: "tag_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_CourseTeachers_course_id",
                table: "CourseTeachers",
                column: "course_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_CourseTeachers_user_id",
                table: "CourseTeachers",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Departments_created_by",
                table: "Departments",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackQuestionOptions_created_by",
                table: "FeedbackQuestionOptions",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackQuestionOptions_feedback_id",
                table: "FeedbackQuestionOptions",
                column: "feedback_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_created_by",
                table: "Feedbacks",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_lesson_id",
                table: "Feedbacks",
                column: "lesson_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackSubmissions_feedback_id",
                table: "FeedbackSubmissions",
                column: "feedback_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackSubmissions_lesson_id",
                table: "FeedbackSubmissions",
                column: "lesson_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackSubmissions_user_id",
                table: "FeedbackSubmissions",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_GeneralSettings_created_by",
                table: "GeneralSettings",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_GroupFiles_created_by",
                table: "GroupFiles",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_GroupFiles_GroupId",
                table: "GroupFiles",
                column: "GroupId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_GroupMembers_group_id",
                table: "GroupMembers",
                column: "group_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_GroupMembers_user_id",
                table: "GroupMembers",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Groups_created_by",
                table: "Groups",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_course_id",
                table: "Lessons",
                column: "course_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_created_by",
                table: "Lessons",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_meeting_id",
                table: "Lessons",
                column: "meeting_id",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_question_set_id",
                table: "Lessons",
                column: "question_set_id",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_section_id",
                table: "Lessons",
                column: "section_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Levels_created_by",
                table: "Levels",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_MeetingReports_meeting_id",
                table: "MeetingReports",
                column: "meeting_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_MeetingReports_user_id",
                table: "MeetingReports",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_created_by",
                table: "Meetings",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_zoom_license_id",
                table: "Meetings",
                column: "zoom_license_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_PhysicalLessonReviews_lesson_id",
                table: "PhysicalLessonReviews",
                column: "lesson_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_PhysicalLessonReviews_user_id",
                table: "PhysicalLessonReviews",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOptions_created_by",
                table: "QuestionOptions",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOptions_question_id",
                table: "QuestionOptions",
                column: "question_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_QuestionPoolQuestions_created_by",
                table: "QuestionPoolQuestions",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_QuestionPoolQuestions_question_id",
                table: "QuestionPoolQuestions",
                column: "question_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_QuestionPoolQuestions_question_pool_id",
                table: "QuestionPoolQuestions",
                column: "question_pool_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_QuestionPools_created_by",
                table: "QuestionPools",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_QuestionPoolTeachers_question_pool_id",
                table: "QuestionPoolTeachers",
                column: "question_pool_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_QuestionPoolTeachers_user_id",
                table: "QuestionPoolTeachers",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Questions_created_by",
                table: "Questions",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSetQuestions_created_by",
                table: "QuestionSetQuestions",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSetQuestions_question_id",
                table: "QuestionSetQuestions",
                column: "question_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSetQuestions_question_pool_question_id",
                table: "QuestionSetQuestions",
                column: "question_pool_question_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSetQuestions_question_set_id",
                table: "QuestionSetQuestions",
                column: "question_set_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSetResults_question_set_id",
                table: "QuestionSetResults",
                column: "question_set_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSetResults_question_set_submission_id",
                table: "QuestionSetResults",
                column: "question_set_submission_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSetResults_user_id",
                table: "QuestionSetResults",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSets_created_by",
                table: "QuestionSets",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSetSubmissionAnswers_created_by",
                table: "QuestionSetSubmissionAnswers",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSetSubmissionAnswers_question_set_question_id",
                table: "QuestionSetSubmissionAnswers",
                column: "question_set_question_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSetSubmissionAnswers_question_set_submission_id",
                table: "QuestionSetSubmissionAnswers",
                column: "question_set_submission_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSetSubmissions_question_set_id",
                table: "QuestionSetSubmissions",
                column: "question_set_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSetSubmissions_user_id",
                table: "QuestionSetSubmissions",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_QuestionTags_created_by",
                table: "QuestionTags",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_QuestionTags_question_id",
                table: "QuestionTags",
                column: "question_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_QuestionTags_tag_id",
                table: "QuestionTags",
                column: "tag_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_user_id",
                table: "RefreshTokens",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Sections_course_id",
                table: "Sections",
                column: "course_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Sections_created_by",
                table: "Sections",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Signature_course_id",
                table: "Signature",
                column: "course_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Signature_created_by",
                table: "Signature",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_SMTPSettings_created_by",
                table: "SMTPSettings",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Tags_created_by",
                table: "Tags",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Users_department_id",
                table: "Users",
                column: "department_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_VideoQueue_lesson_id",
                table: "VideoQueue",
                column: "lesson_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_WatchHistories_course_id",
                table: "WatchHistories",
                column: "course_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_WatchHistories_lesson_id",
                table: "WatchHistories",
                column: "lesson_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_WatchHistories_user_id",
                table: "WatchHistories",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ZoomLicenses_created_by",
                table: "ZoomLicenses",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ZoomSettings_created_by",
                table: "ZoomSettings",
                column: "created_by"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentAttachments_Assignments_assignment_id",
                table: "AssignmentAttachments",
                column: "assignment_id",
                principalTable: "Assignments",
                principalColumn: "id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentAttachments_Users_created_by",
                table: "AssignmentAttachments",
                column: "created_by",
                principalTable: "Users",
                principalColumn: "id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentQuestionOptions_Assignments_assignment_id",
                table: "AssignmentQuestionOptions",
                column: "assignment_id",
                principalTable: "Assignments",
                principalColumn: "id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentQuestionOptions_Users_created_by",
                table: "AssignmentQuestionOptions",
                column: "created_by",
                principalTable: "Users",
                principalColumn: "id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentReviews_Lessons_lesson_id",
                table: "AssignmentReviews",
                column: "lesson_id",
                principalTable: "Lessons",
                principalColumn: "id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentReviews_Users_user_id",
                table: "AssignmentReviews",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Lessons_lesson_id",
                table: "Assignments",
                column: "lesson_id",
                principalTable: "Lessons",
                principalColumn: "id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Users_created_by",
                table: "Assignments",
                column: "created_by",
                principalTable: "Users",
                principalColumn: "id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentSubmissionAttachments_AssignmentSubmissions_assign~",
                table: "AssignmentSubmissionAttachments",
                column: "assignment_submission_id",
                principalTable: "AssignmentSubmissions",
                principalColumn: "id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentSubmissionAttachments_Users_created_by",
                table: "AssignmentSubmissionAttachments",
                column: "created_by",
                principalTable: "Users",
                principalColumn: "id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentSubmissions_Lessons_lesson_id",
                table: "AssignmentSubmissions",
                column: "lesson_id",
                principalTable: "Lessons",
                principalColumn: "id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentSubmissions_Users_user_id",
                table: "AssignmentSubmissions",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Users_created_by",
                table: "Certificates",
                column: "created_by",
                principalTable: "Users",
                principalColumn: "id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_CommentReplies_Comments_comment_id",
                table: "CommentReplies",
                column: "comment_id",
                principalTable: "Comments",
                principalColumn: "id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_CommentReplies_Users_created_by",
                table: "CommentReplies",
                column: "created_by",
                principalTable: "Users",
                principalColumn: "id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Courses_course_id",
                table: "Comments",
                column: "course_id",
                principalTable: "Courses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Users_created_by",
                table: "Comments",
                column: "created_by",
                principalTable: "Users",
                principalColumn: "id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_CourseCertificate_Courses_course_id",
                table: "CourseCertificate",
                column: "course_id",
                principalTable: "Courses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_CourseCertificate_Users_created_by",
                table: "CourseCertificate",
                column: "created_by",
                principalTable: "Users",
                principalColumn: "id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEnrollments_Courses_course_id",
                table: "CourseEnrollments",
                column: "course_id",
                principalTable: "Courses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEnrollments_Lessons_current_lesson_id",
                table: "CourseEnrollments",
                column: "current_lesson_id",
                principalTable: "Lessons",
                principalColumn: "id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEnrollments_Users_user_id",
                table: "CourseEnrollments",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Groups_group_id",
                table: "Courses",
                column: "group_id",
                principalTable: "Groups",
                principalColumn: "id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Levels_level_id",
                table: "Courses",
                column: "level_id",
                principalTable: "Levels",
                principalColumn: "id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Users_created_by",
                table: "Courses",
                column: "created_by",
                principalTable: "Users",
                principalColumn: "id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTags_Tags_tag_id",
                table: "CourseTags",
                column: "tag_id",
                principalTable: "Tags",
                principalColumn: "id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTags_Users_created_by",
                table: "CourseTags",
                column: "created_by",
                principalTable: "Users",
                principalColumn: "id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTeachers_Users_user_id",
                table: "CourseTeachers",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Users_created_by",
                table: "Departments",
                column: "created_by",
                principalTable: "Users",
                principalColumn: "id"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Users_created_by",
                table: "Departments"
            );

            migrationBuilder.DropTable(name: "AssignmentAttachments");

            migrationBuilder.DropTable(name: "AssignmentQuestionOptions");

            migrationBuilder.DropTable(name: "AssignmentReviews");

            migrationBuilder.DropTable(name: "AssignmentSubmissionAttachments");

            migrationBuilder.DropTable(name: "Certificates");

            migrationBuilder.DropTable(name: "CommentReplies");

            migrationBuilder.DropTable(name: "CourseCertificate");

            migrationBuilder.DropTable(name: "CourseEnrollments");

            migrationBuilder.DropTable(name: "CourseTags");

            migrationBuilder.DropTable(name: "CourseTeachers");

            migrationBuilder.DropTable(name: "FeedbackQuestionOptions");

            migrationBuilder.DropTable(name: "FeedbackSubmissions");

            migrationBuilder.DropTable(name: "GeneralSettings");

            migrationBuilder.DropTable(name: "GroupFiles");

            migrationBuilder.DropTable(name: "GroupMembers");

            migrationBuilder.DropTable(name: "Logs");

            migrationBuilder.DropTable(name: "MeetingReports");

            migrationBuilder.DropTable(name: "PhysicalLessonReviews");

            migrationBuilder.DropTable(name: "QuestionOptions");

            migrationBuilder.DropTable(name: "QuestionPoolTeachers");

            migrationBuilder.DropTable(name: "QuestionSetResults");

            migrationBuilder.DropTable(name: "QuestionSetSubmissionAnswers");

            migrationBuilder.DropTable(name: "QuestionTags");

            migrationBuilder.DropTable(name: "RefreshTokens");

            migrationBuilder.DropTable(name: "Settings");

            migrationBuilder.DropTable(name: "Signature");

            migrationBuilder.DropTable(name: "SMTPSettings");

            migrationBuilder.DropTable(name: "VideoQueue");

            migrationBuilder.DropTable(name: "WatchHistories");

            migrationBuilder.DropTable(name: "ZoomSettings");

            migrationBuilder.DropTable(name: "AssignmentSubmissions");

            migrationBuilder.DropTable(name: "Comments");

            migrationBuilder.DropTable(name: "Feedbacks");

            migrationBuilder.DropTable(name: "QuestionSetQuestions");

            migrationBuilder.DropTable(name: "QuestionSetSubmissions");

            migrationBuilder.DropTable(name: "Tags");

            migrationBuilder.DropTable(name: "Assignments");

            migrationBuilder.DropTable(name: "QuestionPoolQuestions");

            migrationBuilder.DropTable(name: "Lessons");

            migrationBuilder.DropTable(name: "QuestionPools");

            migrationBuilder.DropTable(name: "Questions");

            migrationBuilder.DropTable(name: "Meetings");

            migrationBuilder.DropTable(name: "QuestionSets");

            migrationBuilder.DropTable(name: "Sections");

            migrationBuilder.DropTable(name: "ZoomLicenses");

            migrationBuilder.DropTable(name: "Courses");

            migrationBuilder.DropTable(name: "Groups");

            migrationBuilder.DropTable(name: "Levels");

            migrationBuilder.DropTable(name: "Users");

            migrationBuilder.DropTable(name: "Departments");
        }
    }
}

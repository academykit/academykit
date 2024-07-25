using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AssessmentAndTrainigEligibility : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[]
                {
                    "id",
                    "created_by",
                    "created_on",
                    "name",
                    "slug",
                    "is_active",
                    "updated_by",
                    "updated_on"
                },
                values: new object[,]
                {
                    {
                        "7df8d749-6172-482b-b5a1-016fbe478795",
                        "30fcd978-f256-4733-840f-759181bc5e63",
                        new DateTime(2022, 11, 4, 10, 35, 19, 307, DateTimeKind.Utc).AddTicks(3004),
                        "Default Group",
                        "default-group",
                        true, // Assuming "is_active" is a boolean column
                        null,
                        null
                    }
                }
            );
            migrationBuilder.AddColumn<bool>(
                name: "is_shuffle",
                table: "QuestionSets",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false
            );

            migrationBuilder.AddColumn<int>(
                name: "no_of_question",
                table: "QuestionSets",
                type: "int",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<bool>(
                name: "show_all",
                table: "QuestionSets",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "end_date",
                table: "Courses",
                type: "DATETIME",
                nullable: true
            );

            migrationBuilder.AddColumn<bool>(
                name: "is_unlimited_end_date",
                table: "Courses",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "start_date",
                table: "Courses",
                type: "DATETIME",
                nullable: true
            );

            migrationBuilder
                .CreateTable(
                    name: "Assessments",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        slug = table
                            .Column<string>(type: "VARCHAR(270)", maxLength: 270, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        title = table
                            .Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        description = table
                            .Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        retake = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                        assessment_status = table.Column<int>(type: "int", nullable: false),
                        start_date = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        end_date = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        duration = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                        weightage = table.Column<int>(
                            type: "int",
                            nullable: false,
                            defaultValue: 0
                        ),
                        message = table
                            .Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true)
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
                        table.PrimaryKey("PK_Assessments", x => x.id);
                        table.ForeignKey(
                            name: "FK_Assessments_Users_created_by",
                            column: x => x.created_by,
                            principalTable: "Users",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "Skills",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        name = table
                            .Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        is_active = table.Column<bool>(
                            type: "tinyint(1)",
                            nullable: false,
                            defaultValue: true
                        ),
                        remarks = table
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
                        table.PrimaryKey("PK_Skills", x => x.id);
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "TrainingEligibility",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        course_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        eligibility_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        training_eligibility_enum = table
                            .Column<string>(type: "VARCHAR(270)", maxLength: 270, nullable: false)
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
                        table.PrimaryKey("PK_TrainingEligibility", x => x.id);
                        table.ForeignKey(
                            name: "FK_TrainingEligibility_Courses_course_id",
                            column: x => x.course_id,
                            principalTable: "Courses",
                            principalColumn: "id",
                            onDelete: ReferentialAction.Cascade
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "AssessmentQuestion",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        assessment_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        name = table
                            .Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        description = table
                            .Column<string>(type: "varchar(5000)", maxLength: 5000, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        hints = table
                            .Column<string>(type: "varchar(5000)", maxLength: 5000, nullable: true)
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
                        table.PrimaryKey("PK_AssessmentQuestion", x => x.id);
                        table.ForeignKey(
                            name: "FK_AssessmentQuestion_Assessments_assessment_id",
                            column: x => x.assessment_id,
                            principalTable: "Assessments",
                            principalColumn: "id"
                        );
                        table.ForeignKey(
                            name: "FK_AssessmentQuestion_Users_created_by",
                            column: x => x.created_by,
                            principalTable: "Users",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "AssessmentSubmission",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        assessment_id = table
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
                        table.PrimaryKey("PK_AssessmentSubmission", x => x.id);
                        table.ForeignKey(
                            name: "FK_AssessmentSubmission_Assessments_assessment_id",
                            column: x => x.assessment_id,
                            principalTable: "Assessments",
                            principalColumn: "id"
                        );
                        table.ForeignKey(
                            name: "FK_AssessmentSubmission_Users_user_id",
                            column: x => x.user_id,
                            principalTable: "Users",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "EligibilityCreations",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        role = table.Column<int>(type: "int", nullable: false),
                        skill_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        training_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        department_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        group_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        completed_assessment_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        assessment_id = table
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
                        table.PrimaryKey("PK_EligibilityCreations", x => x.id);
                        table.ForeignKey(
                            name: "FK_EligibilityCreations_Assessments_assessment_id",
                            column: x => x.assessment_id,
                            principalTable: "Assessments",
                            principalColumn: "id"
                        );
                        table.ForeignKey(
                            name: "FK_EligibilityCreations_Assessments_completed_assessment_id",
                            column: x => x.completed_assessment_id,
                            principalTable: "Assessments",
                            principalColumn: "id"
                        );
                        table.ForeignKey(
                            name: "FK_EligibilityCreations_Courses_training_id",
                            column: x => x.training_id,
                            principalTable: "Courses",
                            principalColumn: "id"
                        );
                        table.ForeignKey(
                            name: "FK_EligibilityCreations_Departments_department_id",
                            column: x => x.department_id,
                            principalTable: "Departments",
                            principalColumn: "id"
                        );
                        table.ForeignKey(
                            name: "FK_EligibilityCreations_Groups_group_id",
                            column: x => x.group_id,
                            principalTable: "Groups",
                            principalColumn: "id"
                        );
                        table.ForeignKey(
                            name: "FK_EligibilityCreations_Skills_skill_id",
                            column: x => x.skill_id,
                            principalTable: "Skills",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "SkillsCriteria",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        skill_assessment_rule = table.Column<int>(type: "int", nullable: false),
                        percentage = table.Column<decimal>(type: "decimal(20,4)", nullable: false),
                        skill_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        assessment_id = table
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
                        table.PrimaryKey("PK_SkillsCriteria", x => x.id);
                        table.ForeignKey(
                            name: "FK_SkillsCriteria_Assessments_assessment_id",
                            column: x => x.assessment_id,
                            principalTable: "Assessments",
                            principalColumn: "id",
                            onDelete: ReferentialAction.Cascade
                        );
                        table.ForeignKey(
                            name: "FK_SkillsCriteria_Skills_skill_id",
                            column: x => x.skill_id,
                            principalTable: "Skills",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "UserSkills",
                    columns: table => new
                    {
                        Id = table.Column<Guid>(
                            type: "char(36)",
                            nullable: false,
                            collation: "ascii_general_ci"
                        ),
                        SkillId = table.Column<Guid>(
                            type: "char(36)",
                            nullable: false,
                            collation: "ascii_general_ci"
                        ),
                        UserId = table
                            .Column<string>(type: "VARCHAR(50)", nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        SkillsId = table
                            .Column<string>(type: "VARCHAR(50)", nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        CreatedBy = table.Column<Guid>(
                            type: "char(36)",
                            nullable: false,
                            collation: "ascii_general_ci"
                        ),
                        CreatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                        UpdatedBy = table.Column<Guid>(
                            type: "char(36)",
                            nullable: true,
                            collation: "ascii_general_ci"
                        ),
                        UpdatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_UserSkills", x => x.Id);
                        table.ForeignKey(
                            name: "FK_UserSkills_Skills_SkillsId",
                            column: x => x.SkillsId,
                            principalTable: "Skills",
                            principalColumn: "id"
                        );
                        table.ForeignKey(
                            name: "FK_UserSkills_Users_UserId",
                            column: x => x.UserId,
                            principalTable: "Users",
                            principalColumn: "id",
                            onDelete: ReferentialAction.Cascade
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "AssessmentOptions",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        assessment_question_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        option = table
                            .Column<string>(type: "VARCHAR(5000)", maxLength: 5000, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        order = table.Column<int>(type: "int", nullable: false),
                        UserId = table
                            .Column<string>(type: "VARCHAR(50)", nullable: true)
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
                        table.PrimaryKey("PK_AssessmentOptions", x => x.id);
                        table.ForeignKey(
                            name: "FK_AssessmentOptions_AssessmentQuestion_assessment_question_id",
                            column: x => x.assessment_question_id,
                            principalTable: "AssessmentQuestion",
                            principalColumn: "id",
                            onDelete: ReferentialAction.Cascade
                        );
                        table.ForeignKey(
                            name: "FK_AssessmentOptions_Users_UserId",
                            column: x => x.UserId,
                            principalTable: "Users",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "AssessmentResult",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        user_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        assessment_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        assessment_submission_id = table
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
                        table.PrimaryKey("PK_AssessmentResult", x => x.id);
                        table.ForeignKey(
                            name: "FK_AssessmentResult_AssessmentSubmission_assessment_submission_~",
                            column: x => x.assessment_submission_id,
                            principalTable: "AssessmentSubmission",
                            principalColumn: "id"
                        );
                        table.ForeignKey(
                            name: "FK_AssessmentResult_Assessments_assessment_id",
                            column: x => x.assessment_id,
                            principalTable: "Assessments",
                            principalColumn: "id"
                        );
                        table.ForeignKey(
                            name: "FK_AssessmentResult_Users_user_id",
                            column: x => x.user_id,
                            principalTable: "Users",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "AssessmentSubmissionAnswer",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        assessment_submission_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        assessment_question_id = table
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
                        table.PrimaryKey("PK_AssessmentSubmissionAnswer", x => x.id);
                        table.ForeignKey(
                            name: "FK_AssessmentSubmissionAnswer_AssessmentQuestion_assessment_que~",
                            column: x => x.assessment_question_id,
                            principalTable: "AssessmentQuestion",
                            principalColumn: "id",
                            onDelete: ReferentialAction.Cascade
                        );
                        table.ForeignKey(
                            name: "FK_AssessmentSubmissionAnswer_AssessmentSubmission_assessment_s~",
                            column: x => x.assessment_submission_id,
                            principalTable: "AssessmentSubmission",
                            principalColumn: "id"
                        );
                        table.ForeignKey(
                            name: "FK_AssessmentSubmissionAnswer_Users_created_by",
                            column: x => x.created_by,
                            principalTable: "Users",
                            principalColumn: "id"
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentOptions_assessment_question_id",
                table: "AssessmentOptions",
                column: "assessment_question_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentOptions_UserId",
                table: "AssessmentOptions",
                column: "UserId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentQuestion_assessment_id",
                table: "AssessmentQuestion",
                column: "assessment_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentQuestion_created_by",
                table: "AssessmentQuestion",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentResult_assessment_id",
                table: "AssessmentResult",
                column: "assessment_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentResult_assessment_submission_id",
                table: "AssessmentResult",
                column: "assessment_submission_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentResult_user_id",
                table: "AssessmentResult",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Assessments_created_by",
                table: "Assessments",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentSubmission_assessment_id",
                table: "AssessmentSubmission",
                column: "assessment_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentSubmission_user_id",
                table: "AssessmentSubmission",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentSubmissionAnswer_assessment_question_id",
                table: "AssessmentSubmissionAnswer",
                column: "assessment_question_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentSubmissionAnswer_assessment_submission_id",
                table: "AssessmentSubmissionAnswer",
                column: "assessment_submission_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentSubmissionAnswer_created_by",
                table: "AssessmentSubmissionAnswer",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_EligibilityCreations_assessment_id",
                table: "EligibilityCreations",
                column: "assessment_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_EligibilityCreations_completed_assessment_id",
                table: "EligibilityCreations",
                column: "completed_assessment_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_EligibilityCreations_department_id",
                table: "EligibilityCreations",
                column: "department_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_EligibilityCreations_group_id",
                table: "EligibilityCreations",
                column: "group_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_EligibilityCreations_skill_id",
                table: "EligibilityCreations",
                column: "skill_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_EligibilityCreations_training_id",
                table: "EligibilityCreations",
                column: "training_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_SkillsCriteria_assessment_id",
                table: "SkillsCriteria",
                column: "assessment_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_SkillsCriteria_skill_id",
                table: "SkillsCriteria",
                column: "skill_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_TrainingEligibility_course_id",
                table: "TrainingEligibility",
                column: "course_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_UserSkills_SkillsId",
                table: "UserSkills",
                column: "SkillsId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_UserSkills_UserId",
                table: "UserSkills",
                column: "UserId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "AssessmentOptions");

            migrationBuilder.DropTable(name: "AssessmentResult");

            migrationBuilder.DropTable(name: "AssessmentSubmissionAnswer");

            migrationBuilder.DropTable(name: "EligibilityCreations");

            migrationBuilder.DropTable(name: "SkillsCriteria");

            migrationBuilder.DropTable(name: "TrainingEligibility");

            migrationBuilder.DropTable(name: "UserSkills");

            migrationBuilder.DropTable(name: "AssessmentQuestion");

            migrationBuilder.DropTable(name: "AssessmentSubmission");

            migrationBuilder.DropTable(name: "Skills");

            migrationBuilder.DropTable(name: "Assessments");

            migrationBuilder.DropColumn(name: "is_shuffle", table: "QuestionSets");

            migrationBuilder.DropColumn(name: "no_of_question", table: "QuestionSets");

            migrationBuilder.DropColumn(name: "show_all", table: "QuestionSets");

            migrationBuilder.DropColumn(name: "end_date", table: "Courses");

            migrationBuilder.DropColumn(name: "is_unlimited_end_date", table: "Courses");

            migrationBuilder.DropColumn(name: "start_date", table: "Courses");
        }
    }
}

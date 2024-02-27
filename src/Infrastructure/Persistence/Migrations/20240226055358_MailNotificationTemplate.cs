using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MailNotificationTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder
                .CreateTable(
                    name: "MailNotifications",
                    columns: table =>
                        new
                        {
                            id = table
                                .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                                .Annotation("MySql:CharSet", "utf8mb4"),
                            mail_name = table
                                .Column<string>(
                                    type: "VARCHAR(250)",
                                    maxLength: 250,
                                    nullable: false
                                )
                                .Annotation("MySql:CharSet", "utf8mb4"),
                            mail_subject = table
                                .Column<string>(
                                    type: "VARCHAR(500)",
                                    maxLength: 500,
                                    nullable: false
                                )
                                .Annotation("MySql:CharSet", "utf8mb4"),
                            mail_message = table
                                .Column<string>(type: "TEXT", nullable: false)
                                .Annotation("MySql:CharSet", "utf8mb4"),
                            is_active = table.Column<bool>(
                                type: "tinyint(1)",
                                nullable: false,
                                defaultValue: true
                            ),
                            mail_type = table.Column<int>(type: "int", nullable: false),
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
                        table.PrimaryKey("PK_MailNotifications", x => x.id);
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.Sql(
                @"
                    DELETE FROM MailNotifications;
                    
                    INSERT INTO MailNotifications (Id, mail_name, mail_subject, mail_message, Created_By, Created_On, is_active, mail_type)
                    VALUES 
                    ('6d42256e-f88e-4721-9608-44bd9c06f2b6', 'Create User', 'Creating a User', 
                    '<p>Dear {UserName}</p><p>Your Account has been created in the <a target=""_blank"" rel=""noopener noreferrer nofollow"" href=""https://standalone.apps.vurilo.com/"">LMS</a></p><p>Here are the Login details for your LMS account:</p><p>Email: {EmailAddress}</p><p>Password: {Password}</p><p>Please use the above login credentials to access your account.</p><p>{EmailSignature}</p>',
                    '30fcd978-f256-4733-840f-759181bc5e63', NOW(), true, 1),
    
                    ('3e7e85f6-50da-4f6a-b7b0-00b1ad2b9593', 'Resend Email', 'Resend Email', 
                    '<p>Dear {UserName}</p><p>Your Account has been created in the <a target=""_blank"" rel=""noopener noreferrer nofollow"" href=""https://standalone.apps.vurilo.com/"">LMS</a></p><p>Here are the Login details for your LMS account:</p><p>Email: {EmailAddress}</p><p>Password: {Password}</p><p>Please use the above login credentials to access your account.</p><p>{EmailSignature}</p>', 
                    '30fcd978-f256-4733-840f-759181bc5e63', NOW(), true, 2),

                    ('5a1c9d7b-a890-447e-853a-0bc9361783b8', 'Mail Changed', 'Mail Changed', 
                    '<p>Dear {UserName}</p><p>Your Email has been changed in the <a target=""_blank"" rel=""noopener noreferrer nofollow"" href=""https://standalone.apps.vurilo.com/"">LMS</a></p><p>Here are the Login details for your LMS account:</p><p>Email: {EmailAddress}</p><p>Password: {Password}</p><p>Please use the above login credentials to access your account.</p><p>{EmailSignature}</p>', 
                    '30fcd978-f256-4733-840f-759181bc5e63', NOW(), true, 3),

                    ('0410a63f-9345-40b4-bb96-09c7c1433dd7', 'Training Review', 'Training Review Status',
                    '<p>Dear {UserName}</p><p>Training {courseName} is under review. Kindly provide feedback and assessment. Your input is vital for quality assurance. Thank you.</p><p>{EmailSignature}<br></p><p>                  </p>', 
                    '30fcd978-f256-4733-840f-759181bc5e63', NOW(), true, 5),

                    ('26b566c8-c948-4f32-a2b7-a43c5c8641ce', 'Training Enrolled', 'New Enrollment',
                    '<p>Dear {TeacherName}</p><p>A new user has enrolled in your {TrainingName} course. Here are the details:</p><p>Training: {TrainingName} Enrolled User: {userName} User Email:{UserEmail}</p><p>Thank you for your attention to this enrollment. We appreciate your dedication to providing an exceptional learning experience.</p><p>Thank You, {EmailSignature}</p>', 
                    '30fcd978-f256-4733-840f-759181bc5e63', NOW(), true, 6),

                    ('92e97665-16ee-4212-b7fd-b11c85aa57b9', 'Training Reject', 'Training Rejection',
                    '<p>Dear {UserName}</p><p>We regret to inform you that your training {CourseName} has been rejected for the following reason:</p><p>{Message}</p><p>However, we encourage you to make the necessary corrections and adjustments based on the provided feedback. Once you have addressed the identified issues, please resubmit the training program for further review.</p><p>Thank you for your understanding and cooperation.</p><p>{EmailSignature}</p>', 
                    '30fcd978-f256-4733-840f-759181bc5e63', NOW(), true, 7),

                    ('4fefc6c8-2508-475a-9258-2e5c29d93401', 'Group Member Add', 'New group member',
                    '<p>Dear {UserName}</p><p>You have been added to the {GroupName}. Now you can find the Training Materials which has been created for this {GroupName}.</p><p>Link to the group : {GroupLink}</p><p>Thank You, {EmailSignature}</p>', 
                    '30fcd978-f256-4733-840f-759181bc5e63', NOW(), true, 8),

                    ('480797ac-a17b-4e9f-b356-fa9721f20c17', 'Training Publish', 'New training published',
                    '<p>Dear {UserName}</p><p>You have new {TrainingName} training available for the {GroupName} group. Please, go to {GroupName} group or {GroupLink}</p><p>Thank You, {EmailSignature}</p>', 
                    '30fcd978-f256-4733-840f-759181bc5e63', NOW(), true, 9),

                    ('17a02f3a-afe9-4ea8-bb54-56f989c0905b', 'Certificate Issue', 'Certificate Issued',
                    '<p>Dear {UserName}</p><p>We are happy to inform you that your Certificate of Achievement for {TrainingName} has been issued and is now available in your profile on the application.</p><p>Please log in to your account and navigate to your profile to view and download your certificate.</p><p>We hope you find the training helpful.</p><p>Thank You, {EmailSignature}</p>', 
                    '30fcd978-f256-4733-840f-759181bc5e63', NOW(), true, 10);
                    "
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "MailNotifications");
        }
    }
}

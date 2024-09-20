using System.Globalization;
using AcademyKit.Domain.Entities;
using AcademyKit.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AcademyKit.Infrastructure.Persistence.DataSeed;

public static class MailTemplateSeed
{
    private static readonly string UserName_Placeholder = "{UserName}";
    private static readonly string AppUrl_Placeholder = "{AppUrl}";
    private static readonly string EmailAddress_Placeholder = "{EmailAddress}";
    private static readonly string Password_Placeholder = "{Password}";
    private static readonly string EmailSignature_Placeholder = "{EmailSignature}";
    private static readonly string TrainerName_Placeholder = "{TrainerName}";
    private static readonly string TrainingName_Placeholder = "{TrainingName}";
    private static readonly string TrainingSlug_Placeholder = "{TrainingSlug}";
    private static readonly string GroupName_Placeholder = "{GroupName}";
    private static readonly string GroupSlug_Placeholder = "{GroupSlug}";
    private static readonly string AssessmentTitle_Placeholder = "{AssessmentTitle}";
    private static readonly string AssessmentSlug_Placeholder = "{AssessmentSlug}";
    private static readonly string Message_Placeholder = "{Message}";

    private static readonly DateTime CreatedOn = DateTime.ParseExact(
        "2024-03-02 08:55:14",
        "yyyy-MM-dd HH:mm:ss",
        CultureInfo.InvariantCulture
    );

    /// <summary>
    /// SeedAsync method is used to seed the settings data into the database.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static async Task SeedAsync(
        ApplicationDbContext context,
        ILogger logger,
        CancellationToken cancellationToken
    )
    {
        var mailTemplates = GetMailTemplates();
        var existingMailTemplates = await context.MailNotifications.ToListAsync(cancellationToken);

        var newMailTemplates = mailTemplates
            .Where(s => !existingMailTemplates.Exists(e => e.Id == s.Id))
            .ToList();

        var updatedMailTemplates = mailTemplates
            .Where(s => existingMailTemplates.Exists(e => e.Id == s.Id && !e.Equals(s)))
            .ToList();

        if (newMailTemplates.Any())
        {
            await context.MailNotifications.AddRangeAsync(newMailTemplates, cancellationToken);
            logger.LogInformation("New mail templates added.");
        }

        if (updatedMailTemplates.Any())
        {
            foreach (var updatedTemplate in updatedMailTemplates)
            {
                var existingTemplate = existingMailTemplates.First(e => e.Id == updatedTemplate.Id);
                context.Entry(existingTemplate).CurrentValues.SetValues(updatedTemplate);
                logger.LogInformation(
                    "Mail template with ID {templateId} updated.",
                    updatedTemplate.Id
                );
            }
        }

        if (newMailTemplates.Any() || updatedMailTemplates.Any())
        {
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Mail template data seeding completed successfully.");
        }
    }

    /// <summary>
    /// MailNotificationData method is used to get the mail notification data.
    /// </summary>
    /// <returns>The mail notification data.</returns>

    private static List<MailNotification> GetMailTemplates()
    {
        return new List<MailNotification>
        {
            new MailNotification
            {
                Id = Guid.Parse("6d42256e-f88e-4721-9608-44bd9c06f2b6"),
                Name = "Create User",
                Subject = "User Account created",
                Message =
                    $"<p>Dear {UserName_Placeholder} ,<br></p>"
                    + "<p>Your Account has been created in the <a target=\"_blank\" rel=\"noopener noreferrer nofollow\" "
                    + $"href=\"{AppUrl_Placeholder}\">LMS academykit</a></p><p>Here are the Login details for your LMS account:</p>"
                    + $"<p>Email: {EmailAddress_Placeholder}"
                    + $"<br>Password: {Password_Placeholder}</p>"
                    + "<p>Please use the above login credentials to access your account.</p>"
                    + $"<br>{EmailSignature_Placeholder}",
                IsActive = true,
                MailType = MailType.UserCreate,
                CreatedOn = CreatedOn
            },
            new MailNotification
            {
                Id = Guid.Parse("3e7e85f6-50da-4f6a-b7b0-00b1ad2b9593"),
                Name = "Resend Email",
                Subject = "Resend Email",
                Message =
                    $"<p>Dear {UserName_Placeholder},<br></p>"
                    + "<p>Your Account has been created in the <a target=\"_blank\" rel=\"noopener noreferrer nofollow\" "
                    + $"href=\"{AppUrl_Placeholder}\">LMS academykit</a></p><p>Here are the Login details for your LMS account:</p>"
                    + $"<p>Email: {EmailAddress_Placeholder}"
                    + $"<br>Password: {Password_Placeholder}</p>"
                    + "<p>Please use the above login credentials to access your account.</p>"
                    + $"<br>{EmailSignature_Placeholder}",
                IsActive = true,
                MailType = MailType.ResendEmail,
                CreatedOn = CreatedOn
            },
            new MailNotification
            {
                Id = Guid.Parse("5a1c9d7b-a890-447e-853a-0bc9361783b8"),
                Name = "Mail Changed",
                Subject = "Mail Changed",
                Message =
                    $"<p>Dear {UserName_Placeholder},<br></p>"
                    + "<p>Your Email has been changed in the <a target=\"_blank\" rel=\"noopener noreferrer nofollow\" "
                    + $"href=\"{AppUrl_Placeholder}\">LMS</a></p>"
                    + $"<p>Your email has been updated to {EmailAddress_Placeholder}.</p>"
                    + "<p>If you did not request this change, please contact the administrator immediately.</p>"
                    + $"<br>{EmailSignature_Placeholder}",
                IsActive = true,
                MailType = MailType.ChangedEmail,
                CreatedOn = CreatedOn
            },
            new MailNotification
            {
                Id = Guid.Parse("4fefc6c8-2508-475a-9258-2e5c29d93401"),
                Name = "Group Member Add",
                Subject = "New group member",
                Message =
                    $"<p>Dear {UserName_Placeholder},<br></p>"
                    + $"<p>You have been added to the {GroupName_Placeholder}. "
                    + $"Now you can find the Training Materials which has been created for this {GroupName_Placeholder}.</p>"
                    + $"<p>Link to the group : <a href='{AppUrl_Placeholder}/groups/{GroupSlug_Placeholder}'>{GroupName_Placeholder}</a></p>"
                    + $"<br>{EmailSignature_Placeholder}",
                IsActive = true,
                MailType = MailType.GroupMemberAdd,
                CreatedOn = CreatedOn
            },
            new MailNotification
            {
                Id = Guid.Parse("0410a63f-9345-40b4-bb96-09c7c1433dd7"),
                Name = "Training Review",
                Subject = "Training Review Status",
                Message =
                    $"<p>Dear {UserName_Placeholder},<br></p>"
                    + $"<p>Training {TrainingName_Placeholder} is under review. Kindly provide feedback and assessment."
                    + "Your input is vital for quality assurance. <br>Thank you.</p>"
                    + $"<br>{EmailSignature_Placeholder}",
                IsActive = true,
                MailType = MailType.TrainingReview,
                CreatedOn = CreatedOn
            },
            new MailNotification
            {
                Id = Guid.Parse("26b566c8-c948-4f32-a2b7-a43c5c8641ce"),
                Name = "Training Enrolled",
                Subject = "New Enrollment",
                Message =
                    $"<p>Dear {TrainerName_Placeholder},<br></p>"
                    + $"<p>A new user has enrolled in your <a href='{AppUrl_Placeholder}/trainings/{TrainingSlug_Placeholder}'>{TrainingName_Placeholder}</a> course. Here are the details:</p>"
                    + $"<p>Training: {TrainingName_Placeholder} <br> Enrolled User: {UserName_Placeholder} <br> User Email:{EmailAddress_Placeholder}</p>"
                    + "<p>Thank you for your attention to this enrollment. We appreciate your dedication to "
                    + "providing an exceptional learning experience.</p>"
                    + $"<br>{EmailSignature_Placeholder}",
                IsActive = true,
                MailType = MailType.TrainingEnrollment,
                CreatedOn = CreatedOn
            },
            new MailNotification
            {
                Id = Guid.Parse("480797ac-a17b-4e9f-b356-fa9721f20c17"),
                Name = "Training Publish",
                Subject = "New training published",
                Message =
                    $"<p>Dear {UserName_Placeholder},<br></p>"
                    + $"<p>You have new {TrainingName_Placeholder} training available for the {GroupName_Placeholder} group.</p>"
                    + $"<p>Please, go to <a href='{AppUrl_Placeholder}/groups/{GroupSlug_Placeholder}'>{GroupName_Placeholder}</a> group.</p>"
                    + $"<br>{EmailSignature_Placeholder}",
                IsActive = true,
                MailType = MailType.TrainingPublish,
                CreatedOn = CreatedOn
            },
            new MailNotification
            {
                Id = Guid.Parse("92e97665-16ee-4212-b7fd-b11c85aa57b9"),
                Name = "Training Reject",
                Subject = "Training Rejection",
                Message =
                    $"<p>Dear {UserName_Placeholder},<br></p>"
                    + $"<p>We regret to inform you that your training {TrainingName_Placeholder} has been rejected for the following reason:</p>"
                    + $"<br>{Message_Placeholder}</br><p>However, we encourage you to make the necessary corrections and adjustments based on the provided feedback."
                    + "Once you have addressed the identified issues, please resubmit the training program for further review.</p>"
                    + "<p>Thank you for your understanding and cooperation.</p>"
                    + $"<br>{EmailSignature_Placeholder}",
                IsActive = true,
                MailType = MailType.TrainingReject,
                CreatedOn = CreatedOn
            },
            new MailNotification
            {
                Id = Guid.Parse("17a02f3a-afe9-4ea8-bb54-56f989c0905b"),
                Name = "Certificate Issue",
                Subject = "Certificate Issued",
                Message =
                    $"<p>Dear {UserName_Placeholder},<br></p>"
                    + $"<p>We are happy to inform you that your Certificate of Achievement for <a href='{AppUrl_Placeholder}/trainings/{TrainingSlug_Placeholder}'>{TrainingName_Placeholder}</a> "
                    + "has been issued and is now available in your profile on the application.</p><p>Please log in to your account and navigate "
                    + "to your profile to view and download your certificate.</p><p>We hope you find the training helpful.</p>"
                    + $"<br>{EmailSignature_Placeholder}",
                IsActive = true,
                MailType = MailType.CertificateIssue,
                CreatedOn = CreatedOn
            },
            new MailNotification
            {
                Id = Guid.Parse("f3a0b4c5-6d7e-8f9a-0b1c-2d3e4f5a6b7c"),
                Name = "Assessment Review Request",
                Subject = "Assessment Review Request",
                Message =
                    $"<p>Dear {UserName_Placeholder},<br></p>"
                    + $"<p>Assessment <a href='{AppUrl_Placeholder}/assessment/{AssessmentSlug_Placeholder}'>{AssessmentTitle_Placeholder}</a> is requested for review. Thank you.</p>"
                    + $"<br>{EmailSignature_Placeholder}",
                IsActive = true,
                MailType = MailType.AssessmentReview,
                CreatedOn = CreatedOn
            },
            new MailNotification
            {
                Id = Guid.Parse("e2f9b3c4-5d6e-7f8a-9b0c-1d2e3f4a5b6c"),
                Name = "Assessment Published",
                Subject = "Assessment Review Status",
                Message =
                    $"<p>Dear {UserName_Placeholder},<br></p>"
                    + $"<p>Assessment <a href='{AppUrl_Placeholder}/assessment/{AssessmentSlug_Placeholder}'>{AssessmentTitle_Placeholder}</a> published successfully. Thank you.</p>"
                    + $"<br>{EmailSignature_Placeholder}",
                IsActive = true,
                MailType = MailType.AssessmentAccept,
                CreatedOn = CreatedOn
            },
            new MailNotification
            {
                Id = Guid.Parse("d1e8a1b2-3c4d-4e5f-8a6b-7c8d9e0f1a2b"),
                Name = "Assessment Rejection",
                Subject = "Assessment Review Status",
                Message =
                    $"<p>Dear {UserName_Placeholder},<br></p>"
                    + $"<p>We regret to inform you that your Assessment, "
                    + $"<a href='{AppUrl_Placeholder}/assessment/{AssessmentSlug_Placeholder}'>{AssessmentTitle_Placeholder}</a> "
                    + "has been rejected for the following reason:<br>"
                    + $"<br>{Message_Placeholder}<br><p>However, we encourage you to make the necessary corrections and "
                    + "adjustments based on the provided feedback. Once you have addressed the identified issues, "
                    + "please resubmit the assessment for further review.</p>"
                    + "<br><p>Thank you for your understanding and cooperation.</p>"
                    + $"<br>{EmailSignature_Placeholder}",
                IsActive = true,
                MailType = MailType.AssessmentReject,
                CreatedOn = CreatedOn
            },
        };
    }
}

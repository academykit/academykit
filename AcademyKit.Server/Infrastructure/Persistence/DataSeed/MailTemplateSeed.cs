using AcademyKit.Domain.Entities;
using AcademyKit.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AcademyKit.Infrastructure.Persistence.DataSeed;

public class MailTemplateSeed
{
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
            .Where(s => !existingMailTemplates.Any(e => e.Id == s.Id))
            .ToList();

        var updatedMailTemplates = mailTemplates
            .Where(s => existingMailTemplates.Any(e => e.Id == s.Id && !e.Equals(s)))
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
                logger.LogInformation($"Mail template with ID {updatedTemplate.Id} updated.");
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
            // SonarCloud suppression for hard-coded credential warning
            // The Password here is system-generated and not hard-coded
            // NOSONAR
            new MailNotification
            {
                Id = Guid.Parse("6d42256e-f88e-4721-9608-44bd9c06f2b6"),
                Name = "Create User",
                Subject = "User Account created",
                Message =
                    "<p>Dear {UserName},<br></p>"
                    + "<p>Your Account has been created in the <a target=\"_blank\" rel=\"noopener noreferrer nofollow\" "
                    + "href=\"{AppUrl}\">LMS academykit</a></p><p>Here are the Login details for your LMS account:</p>"
                    + "<p>Email: {EmailAddress}"
                    + "<br>Password: {Password}</p>" //NOSONAR
                    + "<p>Please use the above login credentials to access your account.</p>"
                    + "<br>{EmailSignature}",
                IsActive = true,
                MailType = MailType.UserCreate,
                CreatedOn = DateTime.Parse("2024-03-02 08:55:14")
            },
            new MailNotification
            {
                Id = Guid.Parse("3e7e85f6-50da-4f6a-b7b0-00b1ad2b9593"),
                Name = "Resend Email",
                Subject = "Resend Email",
                Message =
                    "<p>Dear {UserName},<br></p>"
                    + "<p>Your Account has been created in the <a target=\"_blank\" rel=\"noopener noreferrer nofollow\" "
                    + "href=\"{AppUrl}\">LMS academykit</a></p><p>Here are the Login details for your LMS account:</p>"
                    + "<p>Email: {EmailAddress}"
                    + "<br>Password: {Password}</p>" //NOSONAR
                    + "<p>Please use the above login credentials to access your account.</p>"
                    + "<br>{EmailSignature}",
                IsActive = true,
                MailType = MailType.ResendEmail,
                CreatedOn = DateTime.Parse("2024-03-02 08:55:14")
            },
            new MailNotification
            {
                Id = Guid.Parse("5a1c9d7b-a890-447e-853a-0bc9361783b8"),
                Name = "Mail Changed",
                Subject = "Mail Changed",
                Message =
                    "<p>Dear {UserName},<br></p>"
                    + "<p>Your Email has been changed in the <a target=\"_blank\" rel=\"noopener noreferrer nofollow\" "
                    + "href=\"{AppUrl}\">LMS</a></p>"
                    + "<p>Your email has been updated to {EmailAddress}.</p>"
                    + "<p>If you did not request this change, please contact the administrator immediately.</p>"
                    + "<br>{EmailSignature}",
                IsActive = true,
                MailType = MailType.ChangedEmail,
                CreatedOn = DateTime.Parse("2024-03-02 08:55:14")
            },
            new MailNotification
            {
                Id = Guid.Parse("4fefc6c8-2508-475a-9258-2e5c29d93401"),
                Name = "Group Member Add",
                Subject = "New group member",
                Message =
                    "<p>Dear {UserName},<br></p>"
                    + "<p>You have been added to the {GroupName}. "
                    + "Now you can find the Training Materials which has been created for this {GroupName}.</p>"
                    + "<p>Link to the group : <a href='{AppUrl}/groups/{GroupSlug}'>{GroupName}</a></p>"
                    + "<br>{EmailSignature}",
                IsActive = true,
                MailType = MailType.GroupMemberAdd,
                CreatedOn = DateTime.Parse("2024-03-02 08:55:14")
            },
            new MailNotification
            {
                Id = Guid.Parse("0410a63f-9345-40b4-bb96-09c7c1433dd7"),
                Name = "Training Review",
                Subject = "Training Review Status",
                Message =
                    "<p>Dear {UserName},<br></p>"
                    + "<p>Training {TrainingName} is under review. Kindly provide feedback and assessment."
                    + "Your input is vital for quality assurance. <br>Thank you.</p>"
                    + "<br>{EmailSignature}",
                IsActive = true,
                MailType = MailType.TrainingReview,
                CreatedOn = DateTime.Parse("2024-03-02 08:55:14")
            },
            new MailNotification
            {
                Id = Guid.Parse("26b566c8-c948-4f32-a2b7-a43c5c8641ce"),
                Name = "Training Enrolled",
                Subject = "New Enrollment",
                Message =
                    "<p>Dear {TrainerName},<br></p>"
                    + "<p>A new user has enrolled in your <a href='{AppUrl}/trainings/{TrainingSlug}'>{TrainingName}</a> course. Here are the details:</p>"
                    + "<p>Training: {TrainingName} <br> Enrolled User: {UserName} <br> User Email:{EmailAddress}</p>"
                    + "<p>Thank you for your attention to this enrollment. We appreciate your dedication to "
                    + "providing an exceptional learning experience.</p>"
                    + "<br>{EmailSignature}",
                IsActive = true,
                MailType = MailType.TrainingEnrollment,
                CreatedOn = DateTime.Parse("2024-03-02 08:55:14")
            },
            new MailNotification
            {
                Id = Guid.Parse("480797ac-a17b-4e9f-b356-fa9721f20c17"),
                Name = "Training Publish",
                Subject = "New training published",
                Message =
                    "<p>Dear {UserName},<br></p>"
                    + "<p>You have new {TrainingName} training available for the {GroupName} group.</p>"
                    + "<p>Please, go to <a href='{AppUrl}/groups/{GroupSlug}'>{GroupName}</a> group.</p>"
                    + "<br>{EmailSignature}",
                IsActive = true,
                MailType = MailType.TrainingPublish,
                CreatedOn = DateTime.Parse("2024-03-02 08:55:14")
            },
            new MailNotification
            {
                Id = Guid.Parse("92e97665-16ee-4212-b7fd-b11c85aa57b9"),
                Name = "Training Reject",
                Subject = "Training Rejection",
                Message =
                    "<p>Dear {UserName},<br></p>"
                    + "<p>We regret to inform you that your training {TrainingName} has been rejected for the following reason:</p>"
                    + "<br>{Message}</br><p>However, we encourage you to make the necessary corrections and adjustments based on the provided feedback."
                    + "Once you have addressed the identified issues, please resubmit the training program for further review.</p>"
                    + "<p>Thank you for your understanding and cooperation.</p>"
                    + "<br>{EmailSignature}",
                IsActive = true,
                MailType = MailType.TrainingReject,
                CreatedOn = DateTime.Parse("2024-03-02 08:55:14")
            },
            new MailNotification
            {
                Id = Guid.Parse("17a02f3a-afe9-4ea8-bb54-56f989c0905b"),
                Name = "Certificate Issue",
                Subject = "Certificate Issued",
                Message =
                    "<p>Dear {UserName},<br></p>"
                    + "<p>We are happy to inform you that your Certificate of Achievement for <a href='{AppUrl}/trainings/{TrainingSlug}'>{TrainingName}</a> "
                    + "has been issued and is now available in your profile on the application.</p><p>Please log in to your account and navigate "
                    + "to your profile to view and download your certificate.</p><p>We hope you find the training helpful.</p>"
                    + "<br>{EmailSignature}",
                IsActive = true,
                MailType = MailType.CertificateIssue,
                CreatedOn = DateTime.Parse("2024-03-02 08:55:14")
            },
            new MailNotification
            {
                Id = Guid.Parse("f3a0b4c5-6d7e-8f9a-0b1c-2d3e4f5a6b7c"),
                Name = "Assessment Review Request",
                Subject = "Assessment Review Request",
                Message =
                    "<p>Dear {UserName},<br></p>"
                    + "<p>Assessment <a href='{AppUrl}/assessment/{AssessmentSlug}'>{AssessmentTitle}</a> is requested for review. Thank you.</p>"
                    + "<br>{EmailSignature}",
                IsActive = true,
                MailType = MailType.AssessmentReview,
                CreatedOn = DateTime.UtcNow
            },
            new MailNotification
            {
                Id = Guid.Parse("e2f9b3c4-5d6e-7f8a-9b0c-1d2e3f4a5b6c"),
                Name = "Assessment Published",
                Subject = "Assessment Review Status",
                Message =
                    "<p>Dear {UserName},<br></p>"
                    + "<p>Assessment <a href='{AppUrl}/assessment/{AssessmentSlug}'>{AssessmentTitle}</a> published successfully. Thank you.</p>"
                    + "<br>{EmailSignature}",
                IsActive = true,
                MailType = MailType.AssessmentAccept,
                CreatedOn = DateTime.UtcNow
            },
            new MailNotification
            {
                Id = Guid.Parse("d1e8a1b2-3c4d-4e5f-8a6b-7c8d9e0f1a2b"),
                Name = "Assessment Rejection",
                Subject = "Assessment Review Status",
                Message =
                    "<p>Dear {UserName},<br></p>"
                    + "<p>We regret to inform you that your Assessment, <a href='{AppUrl}/assessment/{AssessmentSlug}'>{AssessmentTitle}</a> has been rejected for the following reason:<br>"
                    + "<br>{Message}<br><p>However, we encourage you to make the necessary corrections and adjustments based on the provided feedback. "
                    + "Once you have addressed the identified issues, please resubmit the assessment for further review.</p>"
                    + "<br><p>Thank you for your understanding and cooperation.</p>"
                    + "<br>{EmailSignature}",
                IsActive = true,
                MailType = MailType.AssessmentReject,
                CreatedOn = DateTime.UtcNow
            },
        };
    }
}

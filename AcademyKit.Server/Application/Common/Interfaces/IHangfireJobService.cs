using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AcademyKit.Application.Common.Dtos;
using AcademyKit.Domain.Entities;
using Hangfire.Server;

namespace AcademyKit.Application.Common.Interfaces;

/// <summary>
/// Interface for handling Hangfire background jobs related to the Academy Kit application.
/// </summary>
public interface IHangfireJobService
{
    /// <summary>
    /// Sends a course review email to administrators and super administrators.
    /// </summary>
    /// <param name="courseName">The name of the course being reviewed.</param>
    /// <param name="context">The Hangfire performance context.</param>
    Task SendCourseReviewMailAsync(string courseName, PerformContext context = null);

    /// <summary>
    /// Sends a course rejection email to course teachers.
    /// </summary>
    /// <param name="courseId">The ID of the rejected course.</param>
    /// <param name="message">The rejection message.</param>
    /// <param name="context">The Hangfire performance context.</param>
    Task CourseRejectedMailAsync(Guid courseId, string message, PerformContext context = null);

    /// <summary>
    /// Sends an email to a newly created user with their account details.
    /// </summary>
    /// <param name="emailAddress">The user's email address.</param>
    /// <param name="firstName">The user's first name.</param>
    /// <param name="password">The user's initial password.</param>
    /// <param name="companyName">The company name.</param>
    /// <param name="companyNumber">The company number.</param>
    /// <param name="context">The Hangfire performance context.</param>
    Task SendUserCreatedPasswordEmail(
        string emailAddress,
        string firstName,
        string password,
        string companyName,
        string companyNumber,
        PerformContext context = null
    );

    /// <summary>
    /// Sends emails to imported users with their account details.
    /// </summary>
    /// <param name="dtos">A list of user email DTOs containing user information.</param>
    /// <param name="context">The Hangfire performance context.</param>
    Task SendEmailImportedUserAsync(IList<UserEmailDto> dtos, PerformContext context = null);

    /// <summary>
    /// Updates the information of a lesson video, including its duration.
    /// </summary>
    /// <param name="lessonId">The ID of the lesson.</param>
    /// <param name="context">The Hangfire performance context.</param>
    Task LessonVideoUploadedAsync(Guid lessonId, PerformContext context = null);

    /// <summary>
    /// Sends an email to new group members.
    /// </summary>
    /// <param name="groupName">The name of the group.</param>
    /// <param name="groupSlug">The slug of the group.</param>
    /// <param name="userIds">A list of user IDs for the new group members.</param>
    /// <param name="context">The Hangfire performance context.</param>
    Task SendMailNewGroupMember(
        string groupName,
        string groupSlug,
        IList<Guid> userIds,
        PerformContext context = null
    );

    /// <summary>
    /// Sends an email to group members when a new course is published.
    /// </summary>
    /// <param name="groupId">The ID of the group.</param>
    /// <param name="courseName">The name of the course.</param>
    /// <param name="courseSlug">The slug of the course.</param>
    /// <param name="context">The Hangfire performance context.</param>
    Task GroupCoursePublishedMailAsync(
        Guid groupId,
        string courseName,
        string courseSlug,
        PerformContext context = null
    );

    /// <summary>
    /// Sends an email to course teachers when a new user enrolls in their course.
    /// </summary>
    /// <param name="userName">The name of the enrolled user.</param>
    /// <param name="userEmail">The email of the enrolled user.</param>
    /// <param name="courseId">The ID of the course.</param>
    /// <param name="courseName">The name of the course.</param>
    /// <param name="context">The Hangfire performance context.</param>
    Task SendCourseEnrollmentMailAsync(
        string userName,
        string userEmail,
        Guid courseId,
        string courseName,
        PerformContext context = null
    );

    /// <summary>
    /// Sends an email to users when their certificates are issued.
    /// </summary>
    /// <param name="courseName">The name of the course.</param>
    /// <param name="certificateUserIssuedDtos">A list of DTOs containing user information for certificate issuance.</param>
    /// <param name="context">The Hangfire performance context.</param>
    Task SendCertificateIssueMailAsync(
        string courseName,
        IList<CertificateUserIssuedDto> certificateUserIssuedDtos,
        PerformContext context = null
    );

    /// <summary>
    /// Sends an email to enrolled users when new content is added to a course.
    /// </summary>
    /// <param name="courseName">The name of the course.</param>
    /// <param name="courseSlug">The slug of the course.</param>
    /// <param name="context">The Hangfire performance context.</param>
    Task SendLessonAddedMailAsync(
        string courseName,
        string courseSlug,
        PerformContext context = null
    );

    /// <summary>
    /// Sends an email to a user when their account email is updated.
    /// </summary>
    /// <param name="fullName">The full name of the user.</param>
    /// <param name="newEmail">The new email address.</param>
    /// <param name="oldEmail">The old email address.</param>
    /// <param name="context">The Hangfire performance context.</param>
    Task SendEmailChangedMailAsync(
        string fullName,
        string newEmail,
        string oldEmail,
        PerformContext context = null
    );

    /// <summary>
    /// Sends an email to administrators when an assessment is accepted.
    /// </summary>
    /// <param name="assessmentId">The ID of the accepted assessment.</param>
    /// <param name="context">The Hangfire performance context.</param>
    Task SendAssessmentAcceptMailAsync(Guid assessmentId, PerformContext context = null);

    /// <summary>
    /// Sends an email to administrators when an assessment is rejected.
    /// </summary>
    /// <param name="assessmentId">The ID of the rejected assessment.</param>
    /// <param name="context">The Hangfire performance context.</param>
    Task SendAssessmentRejectMailAsync(Guid assessmentId, PerformContext context = null);

    /// <summary>
    /// Sends an email to administrators when an assessment is submitted for review.
    /// </summary>
    /// <param name="assessmentId">The ID of the assessment submitted for review.</param>
    /// <param name="user">The list of users to notify.</param>
    /// <param name="context">The Hangfire performance context.</param>
    Task SendAssessmentReviewMailAsync(
        Guid assessmentId,
        IList<User> user,
        PerformContext context = null
    );
}

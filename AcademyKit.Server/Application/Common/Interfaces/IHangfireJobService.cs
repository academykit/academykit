using AcademyKit.Application.Common.Dtos;
using AcademyKit.Domain.Entities;
using Hangfire.Server;

namespace AcademyKit.Application.Common.Interfaces;

public interface IHangfireJobService
{
    /// <summary>
    /// Handle to send mail to new group member
    /// </summary>
    /// <param name="groupName"> the group name </param>
    /// <param name="groupSlug"> the group slug </param>
    /// <param name="userIds"> the list of <see cref="Guid" /> .</param>
    /// <param name="context"> the instance of <see cref="PerformContext" /> . </param>
    /// <returns> the task complete </returns>
    Task SendMailNewGroupMember(
        string groupName,
        string groupSlug,
        IList<Guid> userIds,
        PerformContext context = null
    );

    /// <summary>
    /// Handle to send group course published mail
    /// </summary>
    /// <param name="groupId"> the group id</param>
    /// <param name="courseName"> the course name </param>
    /// <param name="courseSlug"> the course slug </param>
    /// <param name="context"> the instance of <see cref="PerformContext"/></param>
    /// <returns> the task complete </returns>
    Task GroupCoursePublishedMailAsync(
        Guid groupId,
        string courseName,
        string courseSlug,
        PerformContext context = null
    );

    /// <summary>
    /// Handle to send course rejected mail
    /// </summary>
    /// <param name="courseId"> the course id </param>
    /// <param name="message"> the message </param>
    /// <param name="context"> the instance of <see cref="PerformContext" /> .</param>
    /// <returns> the task complete </returns>
    Task CourseRejectedMailAsync(Guid courseId, string message, PerformContext context);

    /// <summary>
    /// Handle to send course review mail
    /// </summary>
    /// <param name="courseName"> the course name </param>
    /// <param name="context"> the instance of <see cref="PerformContext"/> </param>
    /// <returns> the task complete </returns>
    Task SendCourseReviewMailAsync(string courseName, PerformContext context = null);

    /// <summary>
    /// Handle to send email to imported user async
    /// </summary>
    /// <param name="dtos"> the list of <see cref="UserEmailDto" /> .</param>
    /// <returns> the task complete </returns>
    Task SendEmailImportedUserAsync(IList<UserEmailDto> dtos, PerformContext context = null);

    /// <summary>
    /// Email for account created and password
    /// </summary>
    /// <param name="emailAddress">the email address of the receiver</param>
    /// <param name="firstName">the first name of the receiver</param>
    /// <param name="password">the login password of the receiver</param>
    /// <param name="companyName"> the company name </param>
    /// <returns> the task complete </returns>
    Task SendUserCreatedPasswordEmail(
        string emailAddress,
        string firstName,
        string password,
        string companyName,
        string companyNumber,
        PerformContext context = null
    );

    /// <summary>
    /// Handle to update information of lesson video
    /// </summary>
    /// <param name="lessonId"> the lesson id </param>
    /// <param name="context"> the instance of <see cref="PerformContext"/></param>
    /// <returns> the task complete </returns>
    Task LessonVideoUploadedAsync(Guid lessonId, PerformContext context = null);

    ///<summary>
    ///Handle to send course enrollment mail
    ///</summary>
    ///<param name="userName"> the user name</param>
    ///<param name="userEmail"> the user email</param>
    ///<param name="courseId"> the course id</param>
    ///<param name="courseName"> the course name</param>
    /// <param name="context"> the instance of <see cref="PerformContext" /> . </param>
    ///<returns>the task complete </returns>
    Task SendCourseEnrollmentMailAsync(
        string userName,
        string userEmail,
        Guid courseId,
        string courseName,
        PerformContext context = null
    );

    ///<summary>
    ///Handle to send certificate issued mail
    ///</summary>
    ///<param name="courseName"> the course name</param>
    /// <param name="context"> the instance of <see cref="PerformContext" /> . </param>
    /// <param name="StudentName">the lis of student whose certificate has been issued</param>
    ///<returns>the task complete </returns>
    Task SendCertificateIssueMailAsync(
        string courseName,
        IList<CertificateUserIssuedDto> certificateUserIssuedDtos,
        PerformContext context = null
    );

    /// <summary>
    /// Handle to send lesson added mail
    /// </summary>
    /// <param name="courseName"> the course name </param>
    /// <param name="courseSlug"> the course slug </param>
    /// <param name="context"> the instance of <see cref="PerformContext" /> .</param>
    /// <returns> the task complete </returns>
    Task SendLessonAddedMailAsync(
        string courseName,
        string courseSlug,
        PerformContext context = null
    );

    /// <summary>
    ///
    /// </summary>
    /// <param name="fullName"></param>
    /// <param name="email"></param>
    /// <param name="companyName"></param>
    /// <param name=""></param>
    /// <returns></returns>
    Task AccountUpdatedMailAsync(
        string fullName,
        string email,
        string oldEmail,
        PerformContext context = null
    );

    /// <summary>
    ///
    /// </summary>
    /// <param name="newEmail"></param>
    /// <param name="oldEmail"></param>
    /// <param name="fullName"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    Task SendEmailChangedMailAsync(
        string newEmail,
        string oldEmail,
        string fullName,
        PerformContext context = null
    );

    /// <summary>
    /// Handle to send assessment mail to trainer
    /// </summary>
    /// <param name="assessmentId"> the trainer id </param>
    /// <param name="context"> the instance of <see cref="PerformContext"/> </param>
    /// <returns> the task complete </returns>
    Task SendAssessmentAcceptMailAsync(Guid assessmentId, PerformContext context = null);

    /// <summary>
    /// Handle to send assessment mail to trainer
    /// </summary>
    /// <param name="assessmentId"> the trainer id </param>
    /// <param name="context"> the instance of <see cref="PerformContext"/> </param>
    /// <returns> the task complete </returns>
    Task SendAssessmentRejectMailAsync(Guid assessmentId, PerformContext context = null);

    /// <summary>
    /// Handle to send assessment mail to trainer
    /// </summary>
    /// <param name="assessmentId"> the trainer id </param>
    /// <param name="context"> the instance of <see cref="PerformContext"/> </param>
    /// <returns> the task complete </returns>
    Task SendAssessmentReviewMailAsync(
        Guid assessmentId,
        IList<User> user,
        PerformContext context = null
    );
}

﻿namespace AcademyKit.Application.Common.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.Common.Models.ResponseModels;
    using AcademyKit.Domain.Entities;
    using AcademyKit.Domain.Enums;

    public interface ICourseService : IGenericService<Course, CourseBaseSearchCriteria>
    {
        #region Course CRUD

        /// <summary>
        /// Handle to change course status
        /// </summary>
        /// <param name="model">the instance of <see cref="CourseStatusRequestModel" /> .</param>
        /// <param name="currentUserId">the current id</param>
        /// <returns></returns>
        Task<string> ChangeStatusAsync(CourseStatusRequestModel model, Guid currentUserId);

        /// <summary>
        /// Course Enrollment
        /// </summary>
        /// <param name="identity"> course id or slug</param>
        /// <param name="userId"> the user id</param>
        Task EnrollmentAsync(string identity, Guid userId);

        /// <summary>
        /// Handle to delete course
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns>the task complete</returns>
        Task DeleteCourseAsync(string identity, Guid currentUserId);

        /// <summary>
        /// Handle to update course
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="model">the instance of <see cref="CourseRequestModel"/></param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns></returns>
        Task<Course> UpdateAsync(string identity, CourseRequestModel model, Guid currentUserId);

        /// <summary>
        /// Handle to get user enrollment status
        /// </summary>
        /// <param name="course">the course id</param>
        /// <param name="currentUserId">the current user id</param>
        /// <param name="fetchMembers">the bool value for fetch members</param>
        /// <returns></returns>
        CourseEnrollmentStatus GetUserCourseEnrollmentStatus(Course course, Guid currentUserId);

        /// <summary>
        /// Handle to get user enrollment status
        /// </summary>
        /// <param name="course">the course id</param>
        /// <param name="currentUserId">the current user id</param>
        /// <param name="fetchMembers">the bool value for fetch members</param>
        /// <returns></returns>
        bool GetUserEligibilityStatus(Course course, Guid currentUserId);

        /// <summary>
        /// Handle to get course detail
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns>the instance of <see cref="CourseResponseModel"/></returns>
        Task<CourseResponseModel> GetDetailAsync(string identity, Guid currentUserId);

        /// <summary>
        /// Handle to search group courses
        /// </summary>
        /// <param name="identity">the group id or slug</param>
        /// <param name="criteria">the instance of <see cref="BaseSearchCriteria"/></param>
        /// <returns>the paginated search result</returns>
        Task<SearchResult<Course>> GroupCourseSearchAsync(
            string identity,
            BaseSearchCriteria criteria
        );

        /// <summary>
        /// Handle to update course status
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns></returns>
        Task UpdateCourseStatusAsync(string identity, Guid currentUserId);

        #endregion Course CRUD

        #region Statistics

        /// <summary>
        /// Handle to fetch course lesson statistics
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="currentUserId">the current user id or slug</param>
        /// <param name="criteria"> the instance of <see cref="BaseSearchCriteria"/></param>
        /// <returns>the list of <see cref="LessonStatisticsResponseModel"/></returns>
        Task<SearchResult<LessonStatisticsResponseModel>> LessonStatistics(
            string identity,
            Guid currentUserId,
            BaseSearchCriteria criteria
        );

        /// <summary>
        /// Handle to get course statistics
        /// </summary>
        /// <param name="identity"> the course id or slug </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the instance of <see cref="CourseStatisticsResponseModel" /> . </returns>
        Task<CourseStatisticsResponseModel> GetCourseStatisticsAsync(
            string identity,
            Guid currentUserId
        );

        /// <summary>
        /// Handle to get lesson students report
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="lessonIdentity">the lesson id or slug</param>
        /// <param name="criteria">the instance of <see cref="BaseSearchCriteria"/></param>
        /// <returns>the paginated data</returns>
        Task<SearchResult<LessonStudentResponseModel>> LessonStudentsReport(
            string identity,
            string lessonIdentity,
            BaseSearchCriteria criteria
        );

        /// <summary>
        /// Handle to get lesson students assignment summary report
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="lessonIdentity">the lesson id or slug</param>
        /// <param name="criteria">the instance of <see cref="BaseSearchCriteria"/></param>
        /// <returns>the paginated data</returns>
        Task<AssignmentSummaryResponseModel> AssignmentStudentsReport(
            string identity,
            string lessonIdentity,
            BaseSearchCriteria criteria
        );

        /// <summary>
        /// Handle to get lesson students assignment summary report
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="lessonIdentity">the lesson id or slug</param>
        /// <param name="criteria">the instance of <see cref="BaseSearchCriteria"/></param>
        /// <returns>the paginated data</returns>
        Task<SearchResult<AssignmentSubmissionResponseModel>> AssignmentSubmissionStudentsReport(
            string identity,
            string lessonIdentity,
            BaseSearchCriteria criteria
        );

        /// <summary>
        /// Handle to get lesson students report
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="lessonIdentity">the lesson id or slug</param>
        /// <param name="criteria">the instance of <see cref="BaseSearchCriteria"/></param>
        /// <returns>the paginated data</returns>
        Task<ExamSummaryResponseModel> ExamSummaryReport(
            string identity,
            string lessonIdentity,
            BaseSearchCriteria criteria
        );

        /// <summary>
        /// Handle to get lesson students report
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="lessonIdentity">the lesson id or slug</param>
        /// <param name="criteria">the instance of <see cref="BaseSearchCriteria"/></param>
        /// <returns>the paginated data</returns>
        Task<IList<ExamSubmissionResponseModel>> ExamSubmissionReport(
            string identity,
            string lessonIdentity,
            BaseSearchCriteria criteria
        );

        /// <summary>
        /// Handle to get lesson students report
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="lessonIdentity">the lesson id or slug</param>
        /// <param name="criteria">the instance of <see cref="BaseSearchCriteria"/></param>
        /// <returns>the paginated data</returns>
        Task<SearchResult<StudentCourseStatisticsResponseModel>> StudentStatistics(
            string identity,
            Guid currentUserId,
            BaseSearchCriteria criteria
        );

        /// <summary>
        /// Handle to get student lessons detail
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="userId">the student id</param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns>the list of <see cref="LessonStudentResponseModel"/></returns>
        Task<IList<LessonStudentResponseModel>> StudentLessonsDetail(
            string identity,
            Guid userId,
            Guid currentUserId
        );

        /// <summary>
        /// handel to check role of current user
        /// </summary>
        /// <param name="CurrentUserID">current user id</param>
        /// <returns></returns>
        Task ISSuperAdminAdminOrTrainerAsync(Guid CurrentUserID);

        #endregion Statistics

        #region Dashboard

        // <summary>
        /// Handle to get dashboard stats
        /// </summary>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <param name="currentUserRole">the current logged in user role</param>
        /// <returns>the instance of <see cref="DashboardResponseModel"/></returns>
        Task<DashboardResponseModel> GetDashboardStats(
            Guid currentUserId,
            UserRole currentUserRole
        );

        /// <summary>
        /// Handle to get dashboard courses
        /// </summary>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <param name="currentUserRole">the current logged in user role</param>
        /// <param name="criteria">the instance of <see cref="BaseSearchCriteria"/></param>
        /// <returns>the search result of <see cref="DashboardResponseModel"/></returns>
        Task<SearchResult<DashboardCourseResponseModel>> GetDashboardCourses(
            Guid currentUserId,
            UserRole currentUserRole,
            BaseSearchCriteria criteria
        );

        /// <summary>
        /// Handles to get upcoming lesson
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns> the list of lesson <see cref="Lesson" /> .</returns>

        Task<List<DashboardLessonResponseModel>> GetUpcomingLesson(Guid currentUserId);

        #endregion Dashboard

        #region Certificate
        /// <summary>
        /// Handle to issue the certificate
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="model">the instance of <see cref="CertificateIssueRequestModel"/></param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns>the list of <see cref="CourseCertificateResponseModel"/></returns>
        Task<IList<CourseCertificateIssuedResponseModel>> IssueCertificateAsync(
            string identity,
            CertificateIssueRequestModel model,
            Guid currentUserId
        );

        #endregion Certificate

        #region Signature

        /// <summary>
        /// Handle to get signature
        /// </summary>
        /// <param name="identity"> the course id or slug </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the list of <see cref="SignatureResponseModel" /> . </returns>
        Task<IList<SignatureResponseModel>> GetAllSignatureAsync(
            string identity,
            Guid currentUserId
        );

        /// <summary>
        /// Handle to upload signature
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="model">the instance of <see cref="SignatureRequestModel"/></param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns>the instance of <see cref="SignatureResponseModel"/></returns>
        Task<SignatureResponseModel> InsertSignatureAsync(
            string identity,
            SignatureRequestModel model,
            Guid currentUserId
        );

        /// <summary>
        /// Handle to update signature
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="id">the signature id</param>
        /// <param name="model">the instance of <see cref="SignatureRequestModel"/></param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns>the instance of <see cref="SignatureResponseModel"/></returns>
        Task<SignatureResponseModel> UpdateSignatureAsync(
            string identity,
            Guid id,
            SignatureRequestModel model,
            Guid currentUserId
        );

        /// <summary>
        /// Handle to delete signature
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="id">the signature id</param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns>the task complete</returns>
        Task DeleteSignatureAsync(string identity, Guid id, Guid currentUserId);

        /// <summary>
        /// Handle to insert course certificate detail
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="model">the instance of <see cref="CourseCertificateRequestModel"/></param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns></returns>
        Task<CourseCertificateResponseModel> InsertCertificateDetail(
            string identity,
            CourseCertificateRequestModel model,
            Guid currentUserId
        );

        /// <summary>
        /// Handle to get certificate detail information
        /// </summary>
        /// <param name="identity">the course id or slug </param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns></returns>
        Task<CourseCertificateResponseModel> GetCertificateDetailAsync(
            string identity,
            Guid currentUserId
        );

        /// <summary>
        /// Handle to get user courses list with progress detail
        /// </summary>
        /// <param name="userId">the requested user id</param>
        /// <param name="criteria">the instance of <see cref="BaseSearchCriteria"/></param>
        /// <returns>the search result of <see cref="CourseResponseModel"/></returns>
        Task<SearchResult<CourseResponseModel>> GetUserCourses(
            Guid userId,
            BaseSearchCriteria criteria
        );

        Task<IList<ExamSubmissionResultExportModel>> GetResultsExportAsync(
            string lessonIdentity,
            Guid currentUserId
        );

        #endregion Signature
    }
}

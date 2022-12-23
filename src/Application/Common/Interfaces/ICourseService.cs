namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using System;
    using System.Threading.Tasks;

    public interface ICourseService : IGenericService<Course, CourseBaseSearchCriteria>
    {
        #region Course CRUD

        /// <summary>
        /// Handle to change course status
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="status">the course status</param>
        /// <param name="currentUserId">the current id</param>
        /// <returns></returns>
        Task ChangeStatusAsync(string identity, CourseStatus status, Guid currentUserId);
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
        Task<CourseEnrollmentStatus> GetUserCourseEnrollmentStatus(Course course, Guid currentUserId, bool fetchMembers = false);

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
        Task<SearchResult<Course>> GroupCourseSearchAsync(string identity, BaseSearchCriteria criteria);

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
        /// <returns>the list of <see cref="LessonStatisticsResponseModel"/></returns>
        Task<IList<LessonStatisticsResponseModel>> LessonStatistics(string identity, Guid currentUserId);

        /// <summary>
        /// Handle to get lesson students report
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="lessonIdentity">the lesson id or slug</param>
        /// <param name="criteria">the instance of <see cref="BaseSearchCriteria"/></param>
        /// <returns>the paginated data</returns>
        Task<SearchResult<LessonStudentResponseModel>> LessonStudentsReport(string identity, string lessonIdentity, BaseSearchCriteria criteria);

        /// <summary>
        /// Handle to fetch student course statistics report
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns>the search result</returns>
        Task<SearchResult<StudentCourseStatisticsResponseModel>> StudentStatistics(string identity, Guid currentUserId, BaseSearchCriteria criteria);

        /// <summary>
        /// Handle to get student lessons detail
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="userId">the student id</param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns>the list of <see cref="LessonStudentResponseModel"/></returns>
        Task<IList<LessonStudentResponseModel>> StudentLessonsDetail(string identity, Guid userId, Guid currentUserId);

        #endregion Statistics

        #region Dashboard

        // <summary>
        /// Handle to get dashboard stats
        /// </summary>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <param name="currentUserRole">the current logged in user role</param>
        /// <returns>the instance of <see cref="DashboardResponseModel"/></returns>
        Task<DashboardResponseModel> GetDashboardStats(Guid currentUserId, UserRole currentUserRole);

        /// <summary>
        /// Handle to get dashboard courses
        /// </summary>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <param name="currentUserRole">the current logged in user role</param>
        /// <param name="criteria">the instance of <see cref="BaseSearchCriteria"/></param>
        /// <returns>the search result of <see cref="DashboardResponseModel"/></returns>
        Task<SearchResult<DashboardCourseResponseModel>> GetDashboardCourses(Guid currentUserId, UserRole currentUserRole, BaseSearchCriteria criteria);

        #endregion Dashboard

        #region Certificate

        /// <summary>
        /// Handle to search certificate
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="criteria">the instance of <see cref="CertificateBaseSearchCriteria"/></param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns>the paginated result</returns>
        Task<SearchResult<CourseCertificateResponseModel>> SearchCertificateAsync(string identity, CertificateBaseSearchCriteria criteria, Guid currentUserId);

        /// <summary>
        /// Handle to issue the certificate
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="model">the instance of <see cref="CertificateIssueRequestModel"/></param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns>the list of <see cref="CourseCertificateResponseModel"/></returns>
        Task<IList<CourseCertificateResponseModel>> IssueCertificateAsync(string identity, CertificateIssueRequestModel model, Guid currentUserId);
        /// <summary>
        /// Upload Signature
        /// </summary>
        /// <param name="model">the signature rerquest model<see cref="SignatureFileRequestModel"/></param>
        /// <param name="currentUserId">the Guid of current user</param>
        /// <returns>an instance of <see cref="SignatureResponseModel"/></returns>
        Task<IList<SignatureResponseModel>> UploadSignatureImageFile(SignatureRequestModel model, Guid currentUserId);
        /// <summary>
        /// Retrieve Signatures
        /// </summary>
        /// <param name="model">the signature rerquest model<see cref="SignatureFileRequestModel"/></param>
        /// <returns>List of <see cref="SignatureResponseModel"/></returns>
        Task<IList<SignatureResponseModel>> GetSignatureImageFiles(string courseIdentity, Guid currentUserId);

        #endregion Certificate 
    }
}
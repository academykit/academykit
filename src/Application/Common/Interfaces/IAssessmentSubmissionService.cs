namespace AcademyKit.Application.Common.Interfaces
{
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Application.Common.Models.ResponseModels;
    using AcademyKit.Domain.Entities;

    public interface IAssessmentSubmissionService
        : IGenericService<AssessmentSubmission, BaseSearchCriteria>
    {
        Task AnswerSubmission(
            string identity,
            IList<AssessmentSubmissionRequestModel> answers,
            Guid currentUserId
        );
        Task<SearchResult<AssessmentResultResponseModel>> GetResults(
            BaseSearchCriteria searchCriteria,
            string identity,
            Guid currentUserId
        );

        /// <summary>
        /// Handles to fetch result of a particular student result
        /// </summary>
        /// <param name="identity">the question set id or slug </param>
        /// <param name="userId">the student user id </param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns>the instance of <see cref="StudentResultResponseModel"</returns>
        Task<StudentAssessmentResultResponseModel> GetStudentResult(
            string identity,
            Guid userId,
            Guid currentUserId
        );

        Task<AssessmentUserResultResponseModel> GetResultDetail(
            string identity,
            Guid assessmentSubmissionId,
            Guid currentUserId
        );

        Task<IList<AssessmentResultExportModel>> GetResultsExportAsync(
            string identity,
            Guid currentUserId
        );
    }
}

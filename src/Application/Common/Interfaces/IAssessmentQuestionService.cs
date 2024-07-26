using AcademyKit.Application.Common.Dtos;
using AcademyKit.Application.Common.Models.RequestModels;
using AcademyKit.Application.Common.Models.ResponseModels;
using AcademyKit.Domain.Entities;

namespace AcademyKit.Application.Common.Interfaces
{
    public interface IAssessmentQuestionService
        : IGenericService<AssessmentQuestion, AssessmentQuestionBaseSearchCriteria>
    {
        /// <summary>
        /// Handle to update course
        /// </summary>
        /// <param name="identity">the Feedback id or slug</param>
        /// <param name="model">the instance of <see cref="AssessmentQuestionRequestModel"/> </param>
        /// /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        Task<AssessmentQuestion> UpdateAsync(
            string identity,
            AssessmentQuestionRequestModel model,
            Guid currentUserId
        );
        Task<AssessmentExamResponseModel> GetExamQuestion(
            Assessment existingAssessment,
            Guid currentUserId
        );
    }
}

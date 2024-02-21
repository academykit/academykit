using Lingtren.Application.Common.Dtos;
using Lingtren.Application.Common.Models.RequestModels;
using Lingtren.Application.Common.Models.ResponseModels;
using Lingtren.Domain.Entities;

namespace Lingtren.Application.Common.Interfaces
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

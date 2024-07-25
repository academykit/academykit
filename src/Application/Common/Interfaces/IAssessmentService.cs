namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;

    public interface IAssessmentService : IGenericService<Assessment, AssessmentBaseSearchCriteria>
    {
        Task<Assessment> UpdateAsync(
            Guid identity,
            AssessmentRequestModel model,
            Guid currentUserId,
            Assessment existingAssessment
        );
        Task<string> ChangeStatusAsync(
            AssessmentStatusRequestModel model,
            Guid currentUserId,
            Assessment existingAssessment
        );
        Task DeleteAssessmentAsync(string assessmentIdentity, Guid currentUserId);
        Task<(bool hasCompleted, int RemainingAttempts)> GetAssessmentCriteria(
            Assessment assessment,
            string identity,
            Guid currentUserId
        );
        Task<(bool, IEnumerable<EligibilityCreationResponseModel>)> GetUserEligibilityStatus(
            Assessment Entity,
            Guid currentUserId
        );
    }
}

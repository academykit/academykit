namespace AcademyKit.Application.Common.Interfaces
{
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.Common.Models.ResponseModels;
    using AcademyKit.Domain.Entities;

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

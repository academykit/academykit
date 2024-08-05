namespace AcademyKit.Application.Common.Interfaces
{
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Domain.Entities;
    using AcademyKit.Domain.Enums;

    public interface IQuestionPoolTeacherService
        : IGenericService<QuestionPoolTeacher, QuestionPoolTeacherBaseSearchCriteria>
    {
        /// <summary>
        /// Handle to assign role
        /// </summary>
        /// <param name="model"> the instance of <see cref="QuestionPoolRoleModel" /> .</param>
        /// <returns> the task complete </returns>
        Task AssignRoleAsync(string identity, Guid userId, Guid currentUserId, PoolRole role);
    }
}

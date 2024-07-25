namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;

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

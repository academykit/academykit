namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IAssignmentService : IGenericService<Assignment, AssignmentBaseSearchCriteria>
    {
        /// <summary>
        /// Handle to update course
        /// </summary>
        /// <param name="identity">the assignment id or slug</param>
        /// <param name="model">the instance of <see cref="AssignmentRequestModel"/> </param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        Task<Assignment> UpdateAsync(string identity, AssignmentRequestModel model, Guid currentUserId);

        /// <summary>
        /// Handle to submit assignments by the user
        /// </summary>
        /// <param name="lessonIdentity">the lesson id or slug</param>
        /// <param name="model">the list of <see cref="AssignmentSubmissionRequestModel"/></param>
        /// <param name="currentUserId">the current logged in user</param>
        /// <returns></returns>
        Task AssignmentSubmissionAsync(string lessonIdentity, IList<AssignmentSubmissionRequestModel> model, Guid currentUserId);

        /// <summary>
        /// Handle to fetch student submitted assignment
        /// </summary>
        /// <param name="lessonIdentity">the lesson id or slug</param>
        /// <param name="userId">the user id</param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns>the instance of <see cref="AssignmentSubmissionStudentResponseModel"/></returns>
        Task<AssignmentSubmissionStudentResponseModel> GetStudentSubmittedAssignment(string lessonIdentity, Guid userId, Guid currentUserId);

        /// <summary>
        /// Handle to search assignment
        /// </summary>
        /// <param name="searchCriteria">the instance of <see cref="AssignmentBaseSearchCriteria"/></param>
        /// <returns></returns>
        Task<IList<AssignmentResponseModel>> SearchAsync(AssignmentBaseSearchCriteria searchCriteria);

        /// <summary>
        /// Handle to review user assignment
        /// </summary>
        /// <param name="lessonIdentity">the lesson id or slug</param>
        /// <param name="model">the instance of <see cref="AssignmentReviewRequestModel"/></param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns></returns>
        Task AssignmentReviewAsync(string lessonIdentity, AssignmentReviewRequestModel model, Guid currentUserId);

        /// <summary>
        /// Handle to update user assignment review
        /// </summary>
        /// <param name="lessonIdentity">the lesson id or slug</param>
        /// <param name="id">assignment review id</param>
        /// <param name="model">the instance of <see cref="AssignmentReviewRequestModel"/></param>
        /// <param name="currentUserId">the current logged in user id</param>
        /// <returns></returns>
        Task UpdateAssignmentReviewAsync(string lessonIdentity, Guid id, AssignmentReviewRequestModel model, Guid currentUserId);

        /// <summary>
        /// Handle to delete assignment review
        /// </summary>
        /// <param name="lessonIdentity">the lesson id or slug</param>
        /// <param name="id">the assignment review id</param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        Task DeleteReviewAsync(string lessonIdentity, Guid id, Guid currentUserId);

        /// <summary>
        /// reorder the assignemnt questions
        /// </summary>
        /// <param name="currentUserId">current user id</param>
        /// <param name="lessonIdentity">lesson identity</param>
        /// <param name="ids">list of assignment question id</param>
        /// <returns>Task completed</returns>
        Task ReorderAssignmentQuestionAsync(Guid currentUserId,string lessonIdentity,IList<Guid> ids);
    }
}

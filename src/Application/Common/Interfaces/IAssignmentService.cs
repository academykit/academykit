namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Domain.Entities;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IAssignmentService : IGenericService<Assignment, AssignmentBaseSearchCriteria>
    {
        ///// <summary>
        ///// Handle to submit assignments by the user
        ///// </summary>
        ///// <param name="identity"></param>
        ///// <param name="model"></param>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //Task AssignmentSubmissionAsync(string identity, IList<AssignmentSubmissionRequestModel> model, Guid id);

        /// <summary>
        /// Handle to update course
        /// </summary>
        /// <param name="identity">the assignment id or slug</param>
        /// <param name="model">the instance of <see cref="AssignmentRequestModel"/> </param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        Task<Assignment> UpdateAsync(string identity, AssignmentRequestModel model, Guid currentUserId);
    }
}

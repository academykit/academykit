namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Domain.Entities;
    using Lingtren.Application.Common.Dtos;
    public interface ITagService : IGenericService<Tag, BaseSearchCriteria>
    {
        /// <summary>
        /// Handle to delete the tag
        /// </summary>
        /// <param name="identity"> the id or slug </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the task complete </returns>
        Task DeleteTagAsync(string identity, Guid currentUserId);

        /// <summary>
        /// Handle to create the tag
        /// </summary>
        /// <param name="name"> the tag name </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the instance of <see cref="Tag" /> .</returns>
        Task<Tag> CreateTagAsync(string name, Guid currentUserId);

        /// <summary>
        /// Handle to update the tag
        /// </summary>
        /// <param name="identity"> the id or slug </param>
        /// <param name="name"> the tag name </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the instance of <see cref="Tag" />.</returns>
        Task<Tag> UpdateTagAsync(string identity, string name, Guid currentUserId);
    }
}
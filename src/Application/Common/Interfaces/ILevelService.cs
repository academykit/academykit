namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Domain.Entities;
    public interface ILevelService : IGenericService<Level, BaseSearchCriteria>
    {
        /// <summary>
        /// Handle to delete the level
        /// </summary>
        /// <param name="identity"> the id or slug </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the task complete </returns>
        Task DeleteLevelAsync(string identity, Guid currentUserId);

        /// <summary>
        /// Handle to create the level
        /// </summary>
        /// <param name="name"> the tag name </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the instance of <see cref="Level" /> .</returns>
        Task<Level> CreateLevelAsync(string name, Guid currentUserId);

        /// <summary>
        /// Handle to get levels
        /// </summary>
        /// <returns> the list of <see cref="Level" />.</returns>
        Task<IList<Level>> GetLevelsAsync();

        /// <summary>
        /// Handle to update level async
        /// </summary>
        /// <param name="identity"> the id or slug </param>
        /// <param name="name"> the level name </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the instance of <see cref="Level" />.</returns>
        Task<Level> UpdateLevelAsync(string identity, string name, Guid currentUserId);
    }
}

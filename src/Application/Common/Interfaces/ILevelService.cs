namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Domain.Entities;
    using Lingtren.Application.Common.Dtos;
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
    }
}
namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Domain.Entities;
    public interface ISectionService : IGenericService<Section, BaseSearchCriteria>
    {
        /// <summary>
        /// Handle to delete the section
        /// </summary>
        /// <param name="identity"> the id or slug </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the task complete </returns>
        Task DeleteSectionAsync(string identity, Guid currentUserId);
    }
}
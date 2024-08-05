namespace AcademyKit.Application.Common.Interfaces
{
    using System.Collections.Generic;
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Domain.Entities;

    public interface ISectionService : IGenericService<Section, SectionBaseSearchCriteria>
    {
        /// <summary>
        /// Handle to delete the section
        /// </summary>
        /// <param name="identity"> the course id or slug </param>
        /// <param name="sectionIdentity"> the section id or slug </param>
        /// <param name="currentUserId"> the current user id </param>
        /// <returns> the task complete </returns>
        Task DeleteSectionAsync(string identity, string sectionIdentity, Guid currentUserId);

        /// <summary>
        /// Handle to reorder section
        /// </summary>
        /// <param name="identity">the course id or slug</param>
        /// <param name="ids">the section ids</param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns></returns>
        Task ReorderAsync(string identity, IList<Guid> ids, Guid currentUserId);
    }
}

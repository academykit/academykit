namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Domain.Entities;
    using Lingtren.Application.Common.Dtos;

    public interface ICourseService : IGenericService<Course,BaseSearchCriteria>
    {
         
    }
}
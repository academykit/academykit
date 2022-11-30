namespace Lingtren.Application.Common.Models.RequestModels
{
    public class LessonReorderRequestModel
    {
        public string SectionIdentity { get; set; }
        public IList<Guid> Ids { get; set; }
    }
}

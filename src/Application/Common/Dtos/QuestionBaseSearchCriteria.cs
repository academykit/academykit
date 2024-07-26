namespace AcademyKit.Application.Common.Dtos
{
    using AcademyKit.Domain.Enums;

    public class QuestionBaseSearchCriteria : BaseSearchCriteria
    {
        public string PoolIdentity { get; set; }
        public IList<Guid> Tags { get; set; }
        public QuestionTypeEnum? Type { get; set; }

        public QuestionBaseSearchCriteria() { }
    }
}

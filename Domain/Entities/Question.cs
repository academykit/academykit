namespace AcademyKit.Domain.Entities
{
    using AcademyKit.Domain.Common;
    using AcademyKit.Domain.Enums;

    public class Question : AuditableEntity
    {
        public string Name { get; set; }
        public QuestionTypeEnum Type { get; set; }
        public string Description { get; set; }
        public string Hints { get; set; }
        public User User { get; set; }
        public IList<QuestionOption> QuestionOptions { get; set; }
        public IList<QuestionPoolQuestion> QuestionPoolQuestions { get; set; }
        public IList<QuestionSetQuestion> QuestionSetQuestions { get; set; }
        public IList<QuestionTag> QuestionTags { get; set; }
    }
}

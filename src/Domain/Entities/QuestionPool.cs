namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;

    public class QuestionPool : AuditableEntity
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public bool IsDeleted { get; set; }
        public User User { get; set; }
        public IList<QuestionPoolQuestion> QuestionPoolQuestions { get; set; }
        public IList<QuestionPoolTeacher> QuestionPoolTeachers { get; set; }
    }
}

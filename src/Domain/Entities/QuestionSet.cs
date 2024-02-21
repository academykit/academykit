namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Common;

    public class QuestionSet : AuditableEntity
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Description { get; set; }
        public decimal NegativeMarking { get; set; }

        /// <summary>
        /// Weightage of single question
        /// </summary>
        public decimal QuestionMarking { get; set; }

        /// <summary>
        /// Weightage for pass marked
        /// </summary>
        public decimal PassingWeightage { get; set; }
        public int AllowedRetake { get; set; }
        public int Duration { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsDeleted { get; set; }
        public int NoOfQuestion { get; set; }
        public bool IsShuffle { get; set; }
        public bool ShowAll { get; set; }
        public User User { get; set; }
        public IList<QuestionSetQuestion> QuestionSetQuestions { get; set; }
        public Lesson Lesson { get; set; }
        public IList<QuestionSetResult> QuestionSetResults { get; set; }
        public IList<QuestionSetSubmission> QuestionSetSubmissions { get; set; }
    }
}

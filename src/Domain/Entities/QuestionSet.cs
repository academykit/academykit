namespace Lingtren.Domain.Entities
{
    using Lingtren.Domain.Enums;
    using Lingtren.Domain.Common;

    public class QuestionSet : AuditableEntity
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Description { get; set; }
        public int TotalQuestion { get; set; }
        public decimal NegativeMarking { get; set; }
        public decimal QuestionMarking { get; set; }
        public QuestionSetType QuestionSetType { get; set; }
        public int AllowedRetake { get; set; }
        public int Duration { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsDeleted { get; set; }
        public User User { get; set; }
    }
}
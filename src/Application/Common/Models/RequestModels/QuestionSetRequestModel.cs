namespace Lingtren.Application.Common.Models.RequestModels
{
    public class QuestionSetRequestModel
    {
        public string Name { get; set; }
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
        public int NoOfQuestion { get; set; }
        public bool IsShuffle { get; set; }
        public bool ShowAll { get; set; }
    }
}

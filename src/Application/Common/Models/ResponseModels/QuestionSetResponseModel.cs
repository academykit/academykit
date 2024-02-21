namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;

    public class QuestionSetResponseModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Description { get; set; }
        public decimal NegativeMarking { get; set; }
        public int TotalQuestions { get; set; }
        public decimal TotalMarks { get; set; }

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
        public DateTime? UpdatedOn { get; set; }
        public UserModel User { get; set; }
        public int NoOfQuestion { get; set; }
        public bool IsShuffle { get; set; }
        public bool ShowAll { get; set; }

        public QuestionSetResponseModel(QuestionSet model)
        {
            Id = model.Id;
            Name = model.Name;
            Slug = model.Slug;
            ThumbnailUrl = model.ThumbnailUrl;
            Description = model.Description;
            NegativeMarking = model.NegativeMarking;
            QuestionMarking = model.QuestionMarking;
            PassingWeightage = model.PassingWeightage;
            AllowedRetake = model.AllowedRetake;
            Duration = model.Duration;
            StartTime = model.StartTime;
            EndTime = model.EndTime;
            UpdatedOn = model.UpdatedOn;
            User = model.User != null ? new UserModel(model.User) : new UserModel();
            TotalMarks = model.QuestionSetQuestions?.Count * model.QuestionMarking ?? 0;
            TotalQuestions = model.QuestionSetQuestions?.Count ?? 0;
            IsShuffle = model.IsShuffle;
            ShowAll = model.ShowAll;
            NoOfQuestion = model.NoOfQuestion;
        }
    }
}

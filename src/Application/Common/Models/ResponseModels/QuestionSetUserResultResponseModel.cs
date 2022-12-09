﻿namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Enums;

    public class QuestionSetUserResultResponseModel
    {
        public Guid QuestionSetSubmissionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ThumbnailUrl { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string TotalMarks { get; set; }
        public string NegativeMarks { get; set; }
        public string ObtainedMarks { get; set; }
        public UserModel Teacher { get; set; }
        public UserModel User { get; set; }
        public string Duration { get; set; }
        public string CompleteDuration { get; set; }
        public IList<QuestionSetAnswerResultModel> Results { get; set; }
    }
    public class QuestionSetAnswerResultModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Hints { get; set; }
        public string Attachments { get; set; }
        public QuestionTypeEnum Type { get; set; }
        public IList<QuestionResultOption> Options { get; set; }
        public bool IsCorrect { get; set; }
        public int? OrderNumber { get; set; }
    }
    public class QuestionResultOption
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
        public bool IsCorrect { get; set; }
        public bool IsSelected { get; set; }
    }
}

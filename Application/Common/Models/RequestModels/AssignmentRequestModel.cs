﻿namespace AcademyKit.Application.Common.Models.RequestModels
{
    using AcademyKit.Domain.Enums;

    public class AssignmentRequestModel
    {
        public Guid LessonId { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
        public string Hints { get; set; }
        public QuestionTypeEnum Type { get; set; }
        public IList<string> FileUrls { get; set; }
        public IList<AssignmentQuestionOptionRequestModel> Answers { get; set; }
    }

    public class AssignmentQuestionOptionRequestModel
    {
        public string Option { get; set; }
        public bool IsCorrect { get; set; }
    }
}

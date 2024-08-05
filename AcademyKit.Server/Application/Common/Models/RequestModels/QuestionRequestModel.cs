﻿namespace AcademyKit.Application.Common.Models.RequestModels
{
    using AcademyKit.Domain.Enums;

    public class QuestionRequestModel
    {
        public string Name { get; set; }
        public string Hints { get; set; }
        public QuestionTypeEnum Type { get; set; }
        public IList<Guid> Tags { get; set; }
        public string Description { get; set; }

        public Guid QuestionPoolId { get; set; }
        public IList<QuestionOptionRequestModel> Answers { get; set; }
    }

    public class QuestionOptionRequestModel
    {
        public string Option { get; set; }
        public bool IsCorrect { get; set; }
    }
}

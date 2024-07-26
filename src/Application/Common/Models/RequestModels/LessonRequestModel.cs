﻿namespace AcademyKit.Application.Common.Models.RequestModels
{
    using AcademyKit.Domain.Enums;

    public class LessonRequestModel
    {
        public string SectionIdentity { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string VideoUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public string DocumentUrl { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Order { get; set; }
        public bool IsMandatory { get; set; }
        public LessonType Type { get; set; }
        public MeetingRequestModel Meeting { get; set; }
        public QuestionSetRequestModel QuestionSet { get; set; }
    }
}

﻿namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    public class LessonResponseModel
    {
        public Guid Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string VideoUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public string DocumentUrl { get; set; }
        public int Order { get; set; }
        public int Duration { get; set; }
        public bool IsPreview { get; set; }
        public bool IsMandatory { get; set; }
        public LessonType Type { get; set; }
        public bool IsDeleted { get; set; }
        public CourseStatus Status { get; set; }
        public Guid CourseId { get; set; }
        public string CourseName { get; set; }
        public Guid SectionId { get; set; }
        public string SectionName { get; set; }
        public Guid? MeetingId { get; set; }
        public string MeetingName { get; set; }
        public UserModel User { get; set; }
        public LessonResponseModel(Lesson model)
        {
            Id = model.Id;
            Slug = model.Slug;
            Description = model.Description;
            VideoUrl = model.VideoUrl;
            ThumbnailUrl = model.ThumbnailUrl;
            DocumentUrl = model.DocumentUrl;
            Order = model.Order;
            IsDeleted = model.IsDeleted;
            IsPreview = model.IsPreview;
            IsMandatory = model.IsMandatory;
            IsDeleted = model.IsDeleted;
            Status = model.Status;
            CourseId = model.CourseId;
            CourseName = model.Course?.Name;
            SectionId = model.SectionId;
            SectionName = model.Section?.Name;
            MeetingId = model.MeetingId;
            User = model.User != null ? new UserModel(model.User) : new UserModel();
        }
    }
}
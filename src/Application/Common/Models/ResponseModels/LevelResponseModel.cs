﻿namespace AcademyKit.Application.Common.Models.ResponseModels
{
    using AcademyKit.Domain.Entities;

    public class LevelResponseModel
    {
        public Guid Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public UserModel User { get; set; }

        public LevelResponseModel(Level level)
        {
            Id = level.Id;
            Slug = level.Slug;
            Name = level.Name;
            User = level.User != null ? new UserModel(level.User) : new UserModel();
        }
    }
}

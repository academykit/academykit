namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;
    public class LevelResponseModel
    {
        public Guid Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }

        public LevelResponseModel(Level level)
        {
            Id = level.Id;
            Slug = level.Slug;
            Name = level.Name;
        }
    }
}
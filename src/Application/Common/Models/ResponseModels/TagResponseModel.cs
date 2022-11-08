namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;

    public class TagResponseModel
    {
        public Guid Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public TagResponseModel(Tag tag)
        {
            Id = tag.Id;
            Slug = tag.Slug;
            Name = tag.Name;
            IsActive = tag.IsActive;
        }
    }
}
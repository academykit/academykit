using Lingtren.Domain.Entities;

namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class TrainingResponseModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public TrainingResponseModel(Course model)
        {
            Id = model.Id;
            Name = model.Name;
        }
    }
}

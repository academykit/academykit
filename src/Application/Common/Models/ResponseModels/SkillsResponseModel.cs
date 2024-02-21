namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;

    public class SkillsResponseModel
    {
        public Guid Id { get; set; }
        public string SkillName { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }

        public IList<UserModel> UserModel { get; set; } // Add '?' after IList<UserModel> to make it nullable

        public SkillsResponseModel(Skills model)
        {
            Id = model.Id;
            SkillName = model.SkillName;
            IsActive = model.IsActive;
            IsActive = model.IsActive;
            Description = model.Description;
            Id = model.Id;
            SkillName = model.SkillName;
            IsActive = model.IsActive;
            Description = model.Description;
            UserModel =
                model.UserSkills
                    ?.Select(
                        option =>
                            new UserModel { FullName = option.User.FullName, Id = option.UserId }
                    )
                    .ToList() ?? new List<UserModel>();
        }
    }
}

namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;
    public class SectionResponseModel
    {
        public Guid Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public UserModel User { get; set; }
        public IList<LessonResponseModel> Lessons { get; set; }
        public SectionResponseModel(Section model, bool fetchLesson = false)
        {
            Id = model.Id;
            Slug = model.Slug;
            Name = model.Name;
            User = model.User != null ? new UserModel(model.User) : new UserModel();
            Lessons = new List<LessonResponseModel>();
            if (fetchLesson)
            {
                model.Lessons.ToList().ForEach(item => Lessons.Add(new LessonResponseModel(item)));
            }
        }
    }
}
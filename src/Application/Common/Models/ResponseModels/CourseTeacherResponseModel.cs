namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;
    public class CourseTeacherResponseModel
    {
        public Guid Id { get; set; }

        public Guid CourseId { get; set; }

        public string CourseName { get; set; }

        public UserModel User { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CourseCreatedBy { get; set; }

        public CourseTeacherResponseModel(CourseTeacher entity)
        {
            Id = entity.Id;
            CourseId = entity.CourseId;
            CourseName = entity.Course.Name;
            User = entity.User != null ? new UserModel(entity.User) : new UserModel();
            CreatedOn = entity.CreatedOn;
            CourseCreatedBy = entity.Course.CreatedBy.ToString();
        }
    }
}

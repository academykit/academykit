namespace AcademyKit.Application.Common.Models.ResponseModels
{
    public class QuestionSetResultResponseModel
    {
        public Guid Id { get; set; }
        public Guid QuestionSetId { get; set; }
        public decimal ObtainedMarks { get; set; }
        public UserModel User { get; set; }
    }
}

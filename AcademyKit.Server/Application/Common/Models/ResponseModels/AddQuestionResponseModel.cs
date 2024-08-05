using AcademyKit.Domain.Enums;

namespace AcademyKit.Application.Common.Models.ResponseModels
{
    public class AddQuestionResponseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public QuestionTypeEnum Type { get; set; }
        public string Description { get; set; }
        public string Hints { get; set; }
        public List<QuestionOptionResponseModel> Answers { get; set; }
    }

    public class QuestionOptionResponseModel
    {
        public string Option { get; set; }
        public bool IsCorrect { get; set; }
    }
}

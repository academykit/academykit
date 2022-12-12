namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;

    public class QuestionResponseModel
    {
        public Guid Id { get; set; }
        public Guid? QuestionSetQuestionId { get; set; }
        public Guid? QuestionPoolQuestionId { get; set; }
        public string Name { get; set; }
        public QuestionTypeEnum Type { get; set; }
        public string Description { get; set; }
        public string Hints { get; set; }
        public UserModel User { get; set; }
        public IList<QuestionTagResponseModel> Tags { get; set; }
        public IList<QuestionOptionResponseModel> QuestionOptions { get; set; }

        public QuestionResponseModel(Question question, bool showCorrectAnswer = false, Guid? questionPoolQuestionId = null, Guid? questionSetQuestionId = null, bool showHints = true)
        {
            Id = question.Id;
            QuestionSetQuestionId = questionSetQuestionId;
            QuestionPoolQuestionId = questionPoolQuestionId;
            Name = question.Name;
            Type = question.Type;
            Description = question.Description;
            Hints = showHints ? question.Hints : null;
            QuestionOptions = new List<QuestionOptionResponseModel>();
            Tags = new List<QuestionTagResponseModel>();
            User = question.User != null ? new UserModel(question.User) : new UserModel();
            question.QuestionTags.ToList().ForEach(item => Tags.Add(new QuestionTagResponseModel(item)));
            if (question.QuestionOptions?.Count > 0)
            {
                question.QuestionOptions.ToList().ForEach(x => QuestionOptions.Add(new QuestionOptionResponseModel
                {
                    Id = x.Id,
                    Option = x.Option,
                    IsCorrect = showCorrectAnswer ? x.IsCorrect : null,
                    Order = x.Order
                }));
            }
        }
        public class QuestionOptionResponseModel
        {
            public Guid Id { get; set; }
            public string Option { get; set; }
            public bool? IsCorrect { get; set; }
            public int Order { get; set; }
        }

        public class QuestionTagResponseModel
        {
            public Guid Id { get; set; }
            public Guid TagId { get; set; }
            public string TagName { get; set; }
            public QuestionTagResponseModel(QuestionTag questionTag)
            {
                Id = questionTag.Id;
                TagId = questionTag.TagId;
                TagName = questionTag.Tag?.Name;
            }
        }
    }
}

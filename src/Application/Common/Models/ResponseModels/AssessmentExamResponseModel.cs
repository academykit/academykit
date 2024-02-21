using Lingtren.Application.Common.Dtos;

namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class AssessmentExamResponseModel
    {
        public Guid AssessmentId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int Duration { get; set; }
        public string AssessmentName { get; set; }
        public string Description { get; set; }
        public IList<AssessmentExamQuestionResponseModel> Questions { get; set; }
    }
}

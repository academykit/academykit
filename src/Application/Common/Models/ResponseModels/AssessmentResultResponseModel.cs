namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class AssessmentResultResponseModel
    {
        public Guid Id { get; set; }
        public Guid AssessmentId { get; set; }
        public decimal ObtainedMarks { get; set; }
        public UserModel User { get; set; }
    }
}

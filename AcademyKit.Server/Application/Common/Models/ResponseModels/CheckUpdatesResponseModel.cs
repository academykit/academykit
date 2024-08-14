namespace AcademyKit.Application.Common.Models.ResponseModels
{
    public class CheckUpdatesResponseModel
    {
        public string Latest { get; set; }
        public string Current { get; set; }
        public bool Available { get; set; }
        public string ReleaseNotesUrl { get; set; }
    }
}

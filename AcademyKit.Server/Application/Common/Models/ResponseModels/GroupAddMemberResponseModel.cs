namespace AcademyKit.Application.Common.Models.ResponseModels
{
    using System.Net;

    public class GroupAddMemberResponseModel
    {
        public HttpStatusCode HttpStatusCode { get; set; } = HttpStatusCode.OK;
        public string Message { get; set; }
    }
}

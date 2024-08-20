namespace AcademyKit.Server.Application.Common.Interfaces
{
    public interface IMicrosoftService
    {
        Task<string> GetMicrosoftUserEmail(string accessToken);
    }
}

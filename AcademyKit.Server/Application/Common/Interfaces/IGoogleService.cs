using Microsoft.AspNetCore.Mvc;

namespace AcademyKit.Server.Application.Common.Interfaces
{
    public interface IGoogleService
    {
        /// <summary>
        /// get google user email
        /// </summary>
        /// <param name="accessToken">the instance of <see cref="string"/></param>
        /// <returns></returns>
        Task<string> GetGoogleUserEmail(string accessToken);
    }
}

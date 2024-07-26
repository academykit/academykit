namespace AcademyKit.Application.Common.Interfaces
{
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Application.Common.Models.RequestModels;
    using AcademyKit.Application.Common.Models.ResponseModels;
    using AcademyKit.Domain.Enums;
    using OpenAI.Interfaces;

    public interface IAIService
    {
        /// <summary>
        /// Handle to upload file to s3 bucket
        /// </summary>
        /// <param name="file"> the instance of <see cref="MediaRequestModel" /> .</param>
        /// <returns> the instance of <see cref="MediaFileDto" /> . </returns>
        Task<AiResponseModel> ExerciseFunctionCalling(
            IOpenAIService openAIService,
            AiModelEnum aiModelEnum
        );
    }
}

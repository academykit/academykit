﻿namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;

    using OpenAI.Interfaces;

    public interface IAIService
    {
        /// <summary>
        /// Handle to upload file to s3 bucket
        /// </summary>
        /// <param name="file"> the instance of <see cref="MediaRequestModel" /> .</param>
        /// <returns> the instance of <see cref="MediaFileDto" /> . </returns>
        Task<AiResponseMode> ExerciseFunctionCalling(IOpenAIService openAIService);
    }
}

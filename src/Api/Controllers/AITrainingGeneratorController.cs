using Lingtren.Application.Common.Interfaces;
using Lingtren.Application.Common.Models.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Extensions;
using OpenAI.Interfaces;

namespace Lingtren.Api.Controllers
{
    public class AITrainingGeneratorController : BaseApiController
    {
        private readonly IAIService _aIService;

        public AITrainingGeneratorController(IAIService aIService)
        {
            _aIService = aIService;
        }

        [HttpGet()]
        public async Task<AiResponseModel> Get()
        {
            var builder = new ConfigurationBuilder().AddUserSecrets<Program>();

            IConfiguration configuration = builder.Build();
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped(_ => configuration);

            serviceCollection.AddOpenAIService(settings =>
            {
                settings.ApiKey = "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";
            });
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var sdk = serviceProvider.GetRequiredService<IOpenAIService>();

            var dataResponse = await _aIService.ExerciseFunctionCalling(sdk);
            return dataResponse;
        }
    }
}

using Lingtren.Application.Common.Exceptions;
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
        private readonly IAiKeyService aiKeyService;

        public AITrainingGeneratorController(IAIService aIService, IAiKeyService aiKeyService)
        {
            _aIService = aIService;
            this.aiKeyService = aiKeyService;
        }

        [HttpGet()]
        public async Task<AiResponseModel> Get()
        {
            var builder = new ConfigurationBuilder().AddUserSecrets<Program>();
            var ExistingKey = await aiKeyService.GetFirstOrDefaultAsync().ConfigureAwait(false);
            IConfiguration configuration = builder.Build();
            var serviceCollection = new ServiceCollection();
            if (ExistingKey.Key == null && ExistingKey.IsActive == false)
            {
                throw new ForbiddenException($"doesn't Contains key");
            }
            serviceCollection.AddScoped(_ => configuration);

            serviceCollection.AddOpenAIService(settings =>
            {
                settings.ApiKey = ExistingKey.Key;
            });
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var sdk = serviceProvider.GetRequiredService<IOpenAIService>();

            var dataResponse = await _aIService.ExerciseFunctionCalling(sdk);
            return dataResponse;
        }
    }
}

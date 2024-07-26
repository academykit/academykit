using AcademyKit.Application.Common.Exceptions;
using AcademyKit.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Extensions;
using OpenAI.Interfaces;

namespace AcademyKit.Api.Controllers
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

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var builder = new ConfigurationBuilder().AddUserSecrets<Program>();
                var ExistingKey = await aiKeyService
                    .GetFirstOrDefaultAsync(CurrentUser.Id, false)
                    .ConfigureAwait(false);
                IConfiguration configuration = builder.Build();
                var serviceCollection = new ServiceCollection();
                if (ExistingKey.Key == null || ExistingKey.IsActive == false)
                {
                    throw new ForbiddenException($"Key is missing or inactive");
                }

                serviceCollection.AddScoped(_ => configuration);

                serviceCollection.AddOpenAIService(settings =>
                {
                    settings.ApiKey = ExistingKey.Key;
                });
                var serviceProvider = serviceCollection.BuildServiceProvider();
                var sdk = serviceProvider.GetRequiredService<IOpenAIService>();

                var dataResponse = await _aIService.ExerciseFunctionCalling(
                    sdk,
                    ExistingKey.AiModel
                );
                return Ok(dataResponse); // Assuming successful response should be 200 OK
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, ex.Message); // 403 Forbidden status code
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing the request."); // 500 Internal Server Error status code
            }
        }
    }
}

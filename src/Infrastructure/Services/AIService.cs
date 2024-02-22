namespace Lingtren.Infrastructure.Services
{
    using System.Globalization;

    using System.Threading.Tasks;
    using AngleSharp.Svg.Dom;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using Lingtren.Infrastructure.Common;
    using Lingtren.Infrastructure.Localization;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    using OpenAI.Interfaces;
    using OpenAI.ObjectModels;

    using OpenAI.ObjectModels.RequestModels;

    using OpenAI.Utilities.FunctionCalling;

    public class AIService : BaseService, IAIService
    {
        public AIService(
            IUnitOfWork unitOfWork,
            ILogger<AIService> logger,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
            : base(unitOfWork, logger, localizer) { }

        public async Task<AiResponseModel> ExerciseFunctionCalling(IOpenAIService openAIService)
        {
            var training = new Training();
            var responseAI = new AiResponseModel();
            var getTraining = await _unitOfWork
                .GetRepository<Course>()
                .GetAllAsync()
                .ConfigureAwait(false);
            var trainingList = new List<object>();

            foreach (var item in getTraining)
            {
                var trainingObj = new { Title = item.Name };

                trainingList.Add(trainingObj);
            }

            var jsondata = JsonConvert.SerializeObject(trainingList);

            var req = new ChatCompletionCreateRequest();
            if (trainingList.Count == 0)
            {
                req.Tools = FunctionCallingHelper.GetToolDefinitions<Training>();
                req.Messages = new List<ChatMessage>
                {
                    ChatMessage.FromSystem("You are a helpful assistant."),
                    ChatMessage.FromUser(
                        $"Give me Training Title and its Description. There are no existing training Title for my organization. So suggest me some training Title and Description relating to it."
                    )
                };
            }
            else
            {
                req.Tools = FunctionCallingHelper.GetToolDefinitions<Training>();
                req.Messages = new List<ChatMessage>
                {
                    ChatMessage.FromSystem("You are a helpful assistant."),
                    ChatMessage.FromUser(
                        $"Give me Training Title and its Description. These are some of the existing training Title for my organization: {jsondata}"
                    )
                };
            }
            do
            {
                var reply = await openAIService.ChatCompletion.CreateCompletion(
                    req,
                    Models.ChatGpt3_5Turbo
                );

                if (!reply.Successful)
                {
                    Console.WriteLine(reply.Error?.Message);
                    break;
                }

                var response = reply.Choices.First().Message;

                if (response.ToolCalls != null)
                {
                    foreach (var item in response.ToolCalls)
                    {
                        // Assuming your JSON response contains both title and description
                        var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                            item.FunctionCall.Arguments
                        );

                        if (json.ContainsKey("title"))
                        {
                            responseAI.Title = json["title"];
                        }

                        if (json.ContainsKey("description"))
                        {
                            responseAI.Description = json["description"];
                        }
                    }
                }
                else
                {
                    Console.WriteLine(response.Content);
                }

                req.Messages.Add(response);

                if (response.ToolCalls != null)
                {
                    var functionCall = response.ToolCalls.First().FunctionCall;
                    var result = FunctionCallingHelper.CallFunction<string>(
                        functionCall!,
                        training
                    );
                    response.Content = result.ToString(CultureInfo.CurrentCulture);
                }
            } while (req.Messages.Last().FunctionCall != null);

            return responseAI;
        }

        public class Training
        {
            [FunctionDescription("Title of the training")]
            public string GetTitle(string title)
            {
                return title;
            }

            [FunctionDescription("Description of the training")]
            public string GetDescription(string description)
            {
                return description;
            }
        }
    }
}

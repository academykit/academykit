namespace AcademyKit.Infrastructure.Services
{
    using System.Globalization;
    using System.Threading.Tasks;
    using AcademyKit.Application.Common.Interfaces;
    using AcademyKit.Application.Common.Models.ResponseModels;
    using AcademyKit.Domain.Entities;
    using AcademyKit.Domain.Enums;
    using AcademyKit.Infrastructure.Common;
    using AcademyKit.Infrastructure.Localization;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using OpenAI.Interfaces;
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

        public async Task<AiResponseModel> ExerciseFunctionCalling(
            IOpenAIService openAIService,
            AiModelEnum aiModelEnum
        )
        {
            var training = new Training();
            var responseAI = new AiResponseModel();
            var getTraining = await _unitOfWork
                .GetRepository<Course>()
                .GetAllAsync()
                .ConfigureAwait(false);
            var trainingList = new List<object>();
            var aiModel = "";

            switch (aiModelEnum)
            {
                case AiModelEnum.ChatGpt3_5Turbo:
                    aiModel = "gpt-3.5-turbo";
                    break;
                case AiModelEnum.Gpt_3_5_Turbo:
                    aiModel = "gpt-3.5-turbo";
                    break;
                case AiModelEnum.Gpt_3_5_Turbo_16k:
                    aiModel = "gpt-3.5-turbo-16k";
                    break;
                case AiModelEnum.ChatGpt3_5Turbo0301:
                    aiModel = "gpt-3.5-turbo-0301";
                    break;
                case AiModelEnum.Gpt_3_5_Turbo_0301:
                    aiModel = "gpt-3.5-turbo-0301";
                    break;
                case AiModelEnum.Gpt_3_5_Turbo_0613:
                    aiModel = "gpt-3.5-turbo-0613";
                    break;
                case AiModelEnum.Gpt_3_5_Turbo_1106:
                    aiModel = "gpt-3.5-turbo-1106";
                    break;
                case AiModelEnum.Gpt_3_5_Turbo_16k_0613:
                    aiModel = "gpt-3.5-turbo-16k-0613";
                    break;
                case AiModelEnum.Gpt_3_5_Turbo_Instruct:
                    aiModel = "gpt-3.5-turbo-instruct";
                    break;
                case AiModelEnum.WhisperV1:
                    aiModel = "whisper-v1";
                    break;
                case AiModelEnum.Dall_e_2:
                    aiModel = "dall-e-2";
                    break;
                case AiModelEnum.Dall_e_3:
                    aiModel = "dall-e-3";
                    break;
                case AiModelEnum.Tts_1:
                    aiModel = "tts-1";
                    break;
                case AiModelEnum.Tts_1_hd:
                    aiModel = "tts-1-hd";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(aiModelEnum),
                        aiModelEnum,
                        "Unhandled AI model enum value"
                    );
            }

            foreach (var item in getTraining)
            {
                var trainingObj = new { Title = item.Name };

                trainingList.Add(trainingObj);
            }

            var jsonData = JsonConvert.SerializeObject(trainingList);

            var req = new ChatCompletionCreateRequest();
            if (trainingList.Count == 0)
            {
                req.Tools = FunctionCallingHelper.GetToolDefinitions<Training>();
                req.Messages = new List<ChatMessage>
                {
                    ChatMessage.FromSystem("You are a training content specialist."),
                    ChatMessage.FromUser(
                        $"Your main role is to assist users in developing training materials. When given a topic, you will suggest creative and engaging titles for the training sessions, along with concise, informative descriptions that outline the objectives and content of the training. There are no existing training Title for my organization. So suggest me some training Title and Description in about 500 words relating to it."
                    )
                };
            }
            else
            {
                req.Tools = FunctionCallingHelper.GetToolDefinitions<Training>();
                req.Messages = new List<ChatMessage>
                {
                    ChatMessage.FromSystem("You are a training content specialist."),
                    ChatMessage.FromUser(
                        $"Your main role is to assist users in developing training materials. When given a topic, you will suggest creative and engaging titles for the training sessions, along with concise, informative descriptions that outline the objectives and content of the training. These are some of the existing training Title for my organization: {jsonData}"
                    )
                };
            }
            do
            {
                var reply = await openAIService.ChatCompletion.CreateCompletion(req, aiModel);

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
            public static string GetTitle(string title)
            {
                return title;
            }

            [FunctionDescription("Description of the training")]
            public static string GetDescription(string description)
            {
                return description;
            }
        }
    }
}

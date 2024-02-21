using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OpenAI.Extensions;
using OpenAI.Interfaces;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.Utilities.FunctionCalling;
using static Lingtren.Api.Controllers.AITrainingGeneratiorController.FunctionCallingTestHelpers;

namespace Lingtren.Api.Controllers
{
    public class AITrainingGeneratiorController : BaseApiController
    {
        [HttpGet()]
        public async Task<AiResponseMode> Get()
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

            var dataResponse = await FunctionCallingTestHelpers.ExerciseFunctionCalling(sdk);
            return dataResponse;
        }

        public static class FunctionCallingTestHelpers
        {
            public static async Task<AiResponseMode> ExerciseFunctionCalling(
                IOpenAIService openAIService
            )
            {
                var training = new Training();
                var responseAI = new AiResponseMode();

                var req = new ChatCompletionCreateRequest
                {
                    Tools = FunctionCallingHelper.GetToolDefinitions<Training>(),
                    Messages = new List<ChatMessage>
                    {
                        ChatMessage.FromSystem("You are a helpful assistant."),
                        ChatMessage.FromUser(
                            "Give me training Title and its Description for networking training"
                        )
                    }
                };

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

            public class AiResponseMode
            {
                public string Title { get; set; }
                public string Description { get; set; }
            }
        }
    }
}

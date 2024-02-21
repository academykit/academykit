// <copyright file="QuestionPoolController.cs" company="Vurilo Nepal Pvt. Ltd.">
// Copyright (c) Vurilo Nepal Pvt. Ltd.. All rights reserved.
// </copyright>

namespace Lingtren.Api.Controllers
{
    using FluentValidation;
    using Lingtren.Api.Common;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Localization;
    using LinqKit;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    public class QuestionAddAiController : BaseApiController
    {
        private readonly IQuestionPoolService questionPoolService;

        // private readonly IValidator<IList<QuestionRequestModel>> validator;
        private readonly IStringLocalizer<ExceptionLocalizer> localizer;
        private readonly IQuestionService questionService;
        private readonly IQuestionSetService questionSetService;

        public QuestionAddAiController(
            IQuestionPoolService questionPoolService,
            // IValidator<IList<QuestionRequestModel>> validator,
            IStringLocalizer<ExceptionLocalizer> localizer,
            IQuestionService questionService,
            IQuestionSetService questionSetService
        )
        {
            this.questionPoolService = questionPoolService;
            // this.validator = validator;
            this.localizer = localizer;
            this.questionService = questionService;
            this.questionSetService = questionSetService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAsync(AiQuestionRequestModel aiModel)
        {
            // foreach (var item in model.Questions)
            // {
            //     var transformedQuestions = new List<QuestionRequestModel>();
            // }
            var transformedQuestions = new List<QuestionRequestModel>();

            foreach (var aiQuestion in aiModel.Questions)
            {
                // Transform AiQuestion to QuestionRequestModel
                var transformedQuestion = new QuestionRequestModel
                {
                    Name = aiQuestion.Question,
                    Hints = null, // You mentioned hints can be null
                    Type = QuestionTypeEnum.MultipleChoice, // Assuming type is always 2
                    Description = null, // You mentioned description can be null
                    Answers = new List<QuestionOptionRequestModel>()
                };

                // Transform options and answer
                foreach (var option in aiQuestion.Options)
                {
                    var isCorrect = option == aiQuestion.Answer;
                    transformedQuestion.Answers.Add(
                        new QuestionOptionRequestModel { Option = option, IsCorrect = isCorrect }
                    );
                }

                // Add the transformed question to the list
                transformedQuestions.Add(transformedQuestion);
            }

            // IsSuperAdminOrAdminOrTrainer(CurrentUser.Role);
            // await validator
            //     .ValidateAsync(model, options => options.ThrowOnFailures())
            //     .ConfigureAwait(false);

            var userId = new Guid("30fcd978-f256-4733-840f-759181bc5e63");
            var currentTimeStamp = DateTime.UtcNow;
            var searchResult = await questionPoolService
                .SearchAsync(
                    new BaseSearchCriteria { Search = "Ai Question Pool", CurrentUserId = userId }
                )
                .ConfigureAwait(false);

            var entity = new QuestionPool();
            if (searchResult.Items.Count == 0)
            {
                entity = new QuestionPool
                {
                    Id = Guid.NewGuid(),
                    Name = "Ai Question Pool",
                    CreatedOn = currentTimeStamp,
                    CreatedBy = userId,
                    UpdatedOn = currentTimeStamp,
                    UpdatedBy = userId,
                    QuestionPoolTeachers = new List<QuestionPoolTeacher>(),
                };
                entity.QuestionPoolTeachers.Add(
                    new QuestionPoolTeacher()
                    {
                        Id = Guid.NewGuid(),
                        QuestionPoolId = entity.Id,
                        UserId = userId,
                        Role = PoolRole.Creator,
                        CreatedOn = currentTimeStamp,
                        CreatedBy = userId,
                        UpdatedOn = currentTimeStamp,
                        UpdatedBy = userId,
                    }
                );
                var response = await questionPoolService.CreateAsync(entity).ConfigureAwait(false);
                var questionedResponse = await questionService
                    .BulkAddQuestions(transformedQuestions, userId, entity.Id.ToString())
                    .ConfigureAwait(false);
                // await questionSetService.AddQuestionsAsync(
                //     Slug,
                //     questionedResponse,
                //     userId
                // );
                return new JsonResult(new { Message = "success" });
            }
            else
            {
                var questionedResponse = await questionService
                    .BulkAddQuestions(
                        transformedQuestions,
                        userId,
                        searchResult.Items[0].Id.ToString()
                    )
                    .ConfigureAwait(false);
                // await questionSetService.AddQuestionsAsync(Slug, questionedResponse, userId);
                return new JsonResult(new { Message = "success" });
            }
            // return new JsonResult(new { Message = "success" });
        }
    }
}

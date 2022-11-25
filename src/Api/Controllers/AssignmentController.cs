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
    using Lingtren.Infrastructure.Helpers;
    using LinqKit;
    using Microsoft.AspNetCore.Mvc;

    public class AssignmentController : BaseApiController
    {
        private readonly IAssignmentService _assignmentService;
        private readonly IValidator<AssignmentRequestModel> _validator;
        public AssignmentController(
            IAssignmentService assignmentService,
            IValidator<AssignmentRequestModel> validator)
        {
            _assignmentService = assignmentService;
            _validator = validator;
        }
        /// <summary>
        /// get assignment api
        /// </summary>
        /// <returns> the list of <see cref="AssignmentResponseModel" /> .</returns>
        [HttpGet]
        public async Task<SearchResult<AssignmentResponseModel>> SearchAsync([FromQuery] AssignmentBaseSearchCriteria searchCriteria)
        {
            CommonHelper.ValidateArgumentNotNullOrEmpty(searchCriteria.LessonIdentity, nameof(searchCriteria.LessonIdentity));
            var searchResult = await _assignmentService.SearchAsync(searchCriteria).ConfigureAwait(false);

            var response = new SearchResult<AssignmentResponseModel>
            {
                Items = new List<AssignmentResponseModel>(),
                CurrentPage = searchResult.CurrentPage,
                PageSize = searchResult.PageSize,
                TotalCount = searchResult.TotalCount,
                TotalPage = searchResult.TotalPage,
            };

            searchResult.Items.ForEach(p =>
                 response.Items.Add(new AssignmentResponseModel(p))
             );
            return response;
        }

        /// <summary>
        /// create assignment api
        /// </summary>
        /// <param name="model"> the instance of <see cref="AssignmentRequestModel" />. </param>
        /// <returns> the instance of <see cref="AssignmentRequestModel" /> .</returns>
        [HttpPost]
        public async Task<AssignmentResponseModel> CreateAsync(AssignmentRequestModel model)
        {
            IsTeacherAdmin(CurrentUser.Role);

            await _validator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var currentTimeStamp = DateTime.UtcNow;
            var entity = new Assignment
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Hints = model.Hints,
                Description = model.Description,
                LessonId = model.LessonId,
                Type = model.Type,
                IsActive = true,
                CreatedOn = currentTimeStamp,
                CreatedBy = CurrentUser.Id,
                UpdatedOn = currentTimeStamp,
                UpdatedBy = CurrentUser.Id,
                AssignmentAttachments = new List<AssignmentAttachment>(),
                AssignmentQuestionOptions = new List<AssignmentQuestionOption>()
            };
            if ((model.Type == QuestionTypeEnum.SingleChoice || model.Type == QuestionTypeEnum.MultipleChoice) && model.FileUrls?.Count > 0)
            {
                foreach (var item in model.FileUrls.Select((fileUrl, i) => new { i, fileUrl }))
                {
                    entity.AssignmentAttachments.Add(new AssignmentAttachment
                    {
                        Id = Guid.NewGuid(),
                        AssignmentId = entity.Id,
                        FileUrl = item.fileUrl,
                        Order = item.i + 1,
                        CreatedBy = CurrentUser.Id,
                        CreatedOn = currentTimeStamp,
                        UpdatedBy = CurrentUser.Id,
                        UpdatedOn = currentTimeStamp,
                    });
                }
            }
            if (model.Type == QuestionTypeEnum.SingleChoice || model.Type == QuestionTypeEnum.MultipleChoice)
            {
                foreach (var item in model.Answers.Select((answer, i) => new { i, answer }))
                {
                    entity.AssignmentQuestionOptions.Add(new AssignmentQuestionOption
                    {
                        Id = Guid.NewGuid(),
                        AssignmentId = entity.Id,
                        Order = item.i + 1,
                        Option = item.answer.Option,
                        IsCorrect = item.answer.IsCorrect,
                        CreatedBy = CurrentUser.Id,
                        CreatedOn = currentTimeStamp,
                        UpdatedBy = CurrentUser.Id,
                        UpdatedOn = currentTimeStamp,
                    });
                }
            }
            var response = await _assignmentService.CreateAsync(entity).ConfigureAwait(false);
            return new AssignmentResponseModel(response);
        }

        /// <summary>
        /// get assignment by id or slug
        /// </summary>
        /// <param name="identity"> the assignment id or slug</param>
        /// <returns> the instance of <see cref="AssignmentResponseModel" /> .</returns>
        [HttpGet("{identity}")]
        public async Task<AssignmentResponseModel> Get(string identity)
        {
            var model = await _assignmentService.GetByIdOrSlugAsync(identity).ConfigureAwait(false);
            return new AssignmentResponseModel(model);
        }

        /// <summary>
        /// update assignment api
        /// </summary>
        /// <param name="identity"> id or slug </param>
        /// <param name="model"> the instance of <see cref="AssignmentRequestModel" />. </param>
        /// <returns> the instance of <see cref="AssignmentResponseModel" /> .</returns>
        [HttpPut("{identity}")]
        public async Task<AssignmentResponseModel> UpdateAsync(string identity, AssignmentRequestModel model)
        {
            IsTeacherAdmin(CurrentUser.Role);
            await _validator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var savedEntity = await _assignmentService.UpdateAsync(identity, model, CurrentUser.Id).ConfigureAwait(false);
            return new AssignmentResponseModel(savedEntity);
        }

        /// <summary>
        /// delete assignment api
        /// </summary>
        /// <param name="identity"> id or slug </param>
        /// <returns> the task complete </returns>
        [HttpDelete("{identity}")]
        public async Task<IActionResult> DeleteAsync(string identity)
        {
            IsTeacherAdmin(CurrentUser.Role);
            await _assignmentService.DeleteAsync(identity, CurrentUser.Id).ConfigureAwait(false);
            return Ok(new CommonResponseModel() { Success = true, Message = "Department removed successfully." });
        }
    }
}

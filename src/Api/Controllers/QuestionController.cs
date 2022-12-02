namespace Lingtren.Api.Controllers
{
    using ClosedXML.Excel;
    using FluentValidation;
    using Lingtren.Api.Common;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Infrastructure.Helpers;
    using LinqKit;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/QuestionPool/{identity}/Question")]
    public class QuestionController : BaseApiController
    {
        private readonly IQuestionPoolService _questionPoolService;
        private readonly IQuestionService _questionService;
        private readonly IValidator<QuestionRequestModel> _validator;
        public QuestionController(
            IQuestionPoolService questionPoolService,
            IQuestionService questionService,
            IValidator<QuestionRequestModel> validator)
        {
            _questionPoolService = questionPoolService;
            _questionService = questionService;
            _validator = validator;
        }

        /// <summary>
        /// Search the questions.
        /// </summary>
        /// <param name="searchCriteria">The question search criteria.</param>
        /// <returns>The paginated search result.</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<SearchResult<QuestionResponseModel>> SearchAsync(string identity, [FromQuery] QuestionBaseSearchCriteria searchCriteria)
        {
            CommonHelper.ValidateArgumentNotNullOrEmpty(identity, nameof(identity));
            searchCriteria.CurrentUserId = CurrentUser.Id;
            searchCriteria.PoolIdentity = identity;
            var searchResult = await _questionService.SearchAsync(searchCriteria).ConfigureAwait(false);

            var response = new SearchResult<QuestionResponseModel>
            {
                Items = new List<QuestionResponseModel>(),
                CurrentPage = searchResult.CurrentPage,
                PageSize = searchResult.PageSize,
                TotalCount = searchResult.TotalCount,
                TotalPage = searchResult.TotalPage,
            };

            searchResult.Items.ForEach(p =>
                 response.Items.Add(new QuestionResponseModel(p))
             );
            return response;
        }

        /// <summary>
        /// create question api
        /// </summary>
        /// <param name="identity">the question id or slug</param>
        /// <param name="model"> the instance of <see cref="QuestionRequestModel" />. </param>
        /// <returns> the instance of <see cref="QuestionResponseModel" /> .</returns>
        [HttpPost]
        public async Task<QuestionResponseModel> CreateAsync(string identity, QuestionRequestModel model)
        {
            IsSuperAdminOrAdminOrTrainer(CurrentUser.Role);

            var questionPool = await _questionPoolService.GetByIdOrSlugAsync(identity, currentUserId: CurrentUser.Id).ConfigureAwait(false);
            if (questionPool == null)
            {
                throw new EntityNotFoundException("Question pool not found");
            }

            await _validator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var response = await _questionService.AddAsync(identity, model, CurrentUser.Id).ConfigureAwait(false);
            return new QuestionResponseModel(response);
        }

        /// <summary>
        /// get question api
        /// </summary>
        /// <param name="identity"> the question id or slug</param>
        /// <returns> the instance of <see cref="QuestionResponseModel" /> .</returns>
        [HttpGet("{id}")]
        public async Task<QuestionResponseModel> Get(string identity, Guid id)
        {
            var questionPool = await _questionPoolService.GetByIdOrSlugAsync(identity, currentUserId: CurrentUser.Id).ConfigureAwait(false);
            if (questionPool == null)
            {
                throw new EntityNotFoundException("Question pool not found");
            }

            var model = await _questionService.GetByIdOrSlugAsync(id.ToString(), CurrentUser?.Id).ConfigureAwait(false);
            return new QuestionResponseModel(model);
        }

        /// <summary>
        /// update question api
        /// </summary>
        /// <param name="identity">the question id or slug </param>
        /// <param name="model"> the instance of <see cref="QuestionRequestModel" />. </param>
        /// <returns> the instance of <see cref="QuestionResponseModel" /> .</returns>
        [HttpPut("{id}")]
        public async Task<QuestionResponseModel> UpdateAsync(string identity, Guid id, QuestionRequestModel model)
        {
            IsSuperAdminOrAdminOrTrainer(CurrentUser.Role);

            await _validator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var existing = await _questionService.GetByIdOrSlugAsync(identity, CurrentUser.Id).ConfigureAwait(false);
            var currentTimeStamp = DateTime.UtcNow;

            existing.Id = existing.Id;
            existing.Name = model.Name;
            existing.UpdatedBy = CurrentUser.Id;
            existing.UpdatedOn = currentTimeStamp;

            var savedEntity = await _questionService.UpdateAsync(identity, id, model, CurrentUser.Id).ConfigureAwait(false);
            return new QuestionResponseModel(savedEntity);
        }

        /// <summary>
        /// delete question api
        /// </summary>
        /// <param name="identity">the question id or slug </param>
        /// <returns> the task complete </returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string identity, Guid id)
        {
            IsSuperAdminOrAdminOrTrainer(CurrentUser.Role);

            var questionPool = await _questionPoolService.GetByIdOrSlugAsync(identity, currentUserId: CurrentUser.Id).ConfigureAwait(false);
            if (questionPool == null)
            {
                throw new EntityNotFoundException("Question pool not found");
            }

            await _questionService.DeleteAsync(id.ToString(), CurrentUser.Id).ConfigureAwait(false);
            return Ok(new CommonResponseModel() { Success = true, Message = "Question removed successfully." });
        }

        /// <summary>
        /// Bulk question upload excel format
        /// </summary>
        /// <returns></returns>
        [HttpGet("QuestionUploadExcelFormat")]
        [AllowAnonymous]
        public IActionResult DownloadFormat()
        {
            using var workbook = new XLWorkbook();
            var currentRow = 1;
            var worksheet = workbook.Worksheets.Add("Question");
            worksheet.Cell(currentRow, 1).Value = "Title";
            worksheet.Cell(currentRow, 2).Value = "Tags";
            worksheet.Cell(currentRow, 3).Value = "Type";
            worksheet.Cell(currentRow, 4).Value = "Description";
            worksheet.Cell(currentRow, 5).Value = "Hints";
            worksheet.Cell(currentRow, 6).Value = "OptionStart";
            worksheet.Cell(currentRow, 7).Value = "";
            worksheet.Cell(currentRow, 8).Value = "";
            worksheet.Cell(currentRow, 9).Value = "OptionEnd";
            worksheet.Cell(currentRow, 10).Value = "CorrectAnswer";

            var worksheet2 = workbook.Worksheets.Add("Information");

            worksheet2.Cell(currentRow, 1).Value = "Title";
            worksheet2.Cell(currentRow, 2).Value = "Tags";
            worksheet2.Cell(currentRow, 3).Value = "Type";
            worksheet2.Cell(currentRow, 4).Value = "Description";
            worksheet2.Cell(currentRow, 5).Value = "Hints";
            worksheet2.Cell(currentRow, 6).Value = "OptionStart";
            worksheet2.Cell(currentRow, 7).Value = "";
            worksheet2.Cell(currentRow, 8).Value = "";
            worksheet2.Cell(currentRow, 9).Value = "OptionEnd";
            worksheet2.Cell(currentRow, 10).Value = "CorrectAnswer";

            currentRow = 2;
            worksheet2.Cell(currentRow, 1).Value = "Sample Question 1";
            worksheet2.Cell(currentRow, 2).Value = "Sample Question 1";
            worksheet2.Cell(currentRow, 3).Value = "0/1";
            worksheet2.Cell(currentRow, 4).Value = "Question Description";
            worksheet2.Cell(currentRow, 5).Value = "Question Hints";
            worksheet2.Cell(currentRow, 6).Value = "Option 1";
            worksheet2.Cell(currentRow, 7).Value = "Option 2";
            worksheet2.Cell(currentRow, 8).Value = "Option 3";
            worksheet2.Cell(currentRow, 9).Value = "Option 4";
            worksheet2.Cell(currentRow, 10).Value = "F,H";

            worksheet2.Cell(4, 1).Value = "Title";
            worksheet2.Cell(4, 3).Value = "Question title is input in title column.";

            worksheet2.Cell(5, 1).Value = "Tags";
            worksheet2.Cell(5, 3).Value = "Question tags is input in tag column. Like (Biology,Nepali,Math). (Optional Field)";

            worksheet2.Cell(6, 1).Value = "Type";
            worksheet2.Cell(6, 3).Value = "0 is for multiple choice option and 1 is for single choice option. For now we are only accepting multiple choice option so please kindly enter 0";

            worksheet2.Cell(7, 1).Value = "Description";
            worksheet2.Cell(7, 3).Value = "Description of question is inputted here. Both text and image file valid for description.  (Optional Field).";

            worksheet2.Cell(8, 1).Value = "Hints";
            worksheet2.Cell(8, 3).Value = "Hints for question. (Optional Field).";

            worksheet2.Cell(9, 1).Value = "OptionStart/OptionEnd";
            worksheet2.Cell(9, 3).Value = "In between OptionStart and OptionEnd Column, users are requested to input the options for questions. Both text and image file is valid.";

            worksheet2.Cell(10, 1).Value = "CorrectAnswer";
            worksheet2.Cell(10, 3).Value = "Column name between OptionStart and OptionEnd like (F,G,H) is inputted for correct options.";

            worksheet2.Cell(10, 1).Value = "For Image File Input";
            worksheet2.Cell(10, 3).Value = "Users are requested to create images/ folder and save image file in images folder. In the section, " +
                                                "where image file is valid users are requested to input image name with extension like (pic1.png,pic2.jpeg). " +
                                                "And upload file as zip with combination of .xlsx file and images/ folder ";

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();
            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "questions.xlsx");
        }
    }
}

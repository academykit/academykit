namespace Lingtren.Api.Controllers
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/questionSet/{identity}/results")]
    public class QuestionSetResultController : BaseApiController
    {
        private readonly IQuestionSetService _questionSetService;
        public QuestionSetResultController(
            IQuestionSetService questionSetService)
        {
            _questionSetService = questionSetService;
        }

        /// <summary>
        /// get results api
        /// </summary>
        /// <param name="identity"the question set id or slug></param>
        /// <returns>the list of <see cref="QuestionSetResultResponseModel"/></returns>
        [HttpGet]
        public async Task<SearchResult<QuestionSetResultResponseModel>> GetResults([FromQuery] BaseSearchCriteria searchCriteria, string identity)
                                => await _questionSetService.GetResults(searchCriteria, identity, CurrentUser.Id).ConfigureAwait(false);

        /// <summary>
        /// get particular user results
        /// </summary>
        /// <param name="identity"the question set id or slug></param>
        /// <returns>the list of <see cref="QuestionSetResultResponseModel"/></returns>
        [HttpGet("{userId}")]
        public async Task<StudentResultResponseModel> GetStudentResults(string identity, Guid userId)
                                => await _questionSetService.GetStudentResult(identity, userId, CurrentUser.Id).ConfigureAwait(false);

        /// <summary>
        /// get result detail api
        /// </summary>
        /// <param name="identity">the question set id or slug</param>
        /// <param name="questionSetSubmissionId">the question set submission id</param>
        /// <returns></returns>
        [HttpGet("{questionSetSubmissionId}")]
        public async Task<QuestionSetUserResultResponseModel> GetResultDetail(string identity, Guid questionSetSubmissionId) => await _questionSetService.GetResultDetail(identity, questionSetSubmissionId, CurrentUser.Id).ConfigureAwait(false);
    }
}

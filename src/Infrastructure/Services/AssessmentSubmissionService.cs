namespace Lingtren.Infrastructure.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;

    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using Lingtren.Infrastructure.Common;
    using Lingtren.Infrastructure.Localization;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using LinqKit;
    using Microsoft.AspNetCore.Http;
    using Lingtren.Domain.Enums;

    public class AssessmentSubmissionService
        : BaseGenericService<AssessmentSubmission, BaseSearchCriteria>,
            IAssessmentSubmissionService
    {
        public AssessmentSubmissionService(
            IUnitOfWork unitOfWork,
            ILogger<AssessmentSubmissionService> logger,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
            : base(unitOfWork, logger, localizer) { }

        public async Task AnswerSubmission(
            string identity,
            IList<AssessmentSubmissionRequestModel> answers,
            Guid currentUserId
        )
        {
            var currentTimeStamp = DateTime.UtcNow;
            var isSubmissionError = false;

            var assessment = await _unitOfWork
                .GetRepository<Assessment>()
                .GetFirstOrDefaultAsync(
                    predicate: x => x.Id.ToString() == identity || x.Slug == identity
                )
                .ConfigureAwait(false);
            if (assessment == null)
            {
                _logger.LogWarning(
                    "assessment not found with identity: {identity} for user with id : {currentUserId}.",
                    identity,
                    currentUserId
                );
                throw new EntityNotFoundException(_localizer.GetString("AssessmentNotFound"));
            }

            IList<AssessmentSubmissionAnswer> assessmentSubmissionAnswers =
                new List<AssessmentSubmissionAnswer>();

            var assessmentSubmission = await _unitOfWork
                .GetRepository<AssessmentSubmission>()
                .GetFirstOrDefaultAsync(
                    predicate: p => p.AssessmentId == assessment.Id && p.EndTime == default,
                    orderBy: q => q.OrderByDescending(p => p.StartTime)
                )
                .ConfigureAwait(false);

            if (assessmentSubmission == null)
            {
                _logger.LogWarning(
                    "Assessment submission not found with id: {AssessmentId} for user id: {currentUserId}.",
                    assessmentSubmission,
                    currentUserId
                );
                throw new EntityNotFoundException(_localizer.GetString("SubmissionNotFound"));
            }

            assessmentSubmission.EndTime = DateTime.Now;
            assessmentSubmission.IsSubmissionError = isSubmissionError;
            assessmentSubmission.CreatedBy = currentUserId;
            _unitOfWork.GetRepository<AssessmentSubmission>().Update(assessmentSubmission);
            foreach (var item in answers)
            {
                var existingQuestion = await _unitOfWork
                    .GetRepository<AssessmentQuestion>()
                    .GetFirstOrDefaultAsync(
                        predicate: p => p.Id == item.AssessmentQuestionId,
                        include: src => src.Include(x => x.AssessmentOptions)
                    )
                    .ConfigureAwait(false);

                if (existingQuestion != null)
                {
                    var answerIds = existingQuestion.AssessmentOptions
                        .Where(x => x.IsCorrect)
                        .Select(x => x.Id);
                    var isCorrect = answerIds
                        .OrderBy(x => x)
                        .ToList()
                        .SequenceEqual(item.Answers.OrderBy(x => x).ToList());
                    assessmentSubmissionAnswers.Add(
                        new AssessmentSubmissionAnswer
                        {
                            Id = Guid.NewGuid(),
                            AssessmentQuestionId = item.AssessmentQuestionId,
                            AssessmentSubmissionId = assessmentSubmission.Id,
                            SelectedAnswers = string.Join(",", item.Answers),
                            IsCorrect = isCorrect,
                            CreatedBy = currentUserId,
                            CreatedOn = currentTimeStamp
                        }
                    );
                }
            }

            await _unitOfWork
                .GetRepository<AssessmentSubmissionAnswer>()
                .InsertAsync(assessmentSubmissionAnswers)
                .ConfigureAwait(false);
            var AssessmentResult = new AssessmentResult();
            if (!isSubmissionError)
            {
                AssessmentResult = new AssessmentResult()
                {
                    Id = Guid.NewGuid(),
                    UserId = currentUserId,
                    AssessmentSubmissionId = assessmentSubmission.Id,
                    AssessmentId = assessment.Id,
                    TotalMark = 0,
                    NegativeMark = 0,
                    CreatedBy = currentUserId,
                    CreatedOn = currentTimeStamp,
                    UpdatedBy = currentUserId,
                    UpdatedOn = currentTimeStamp,
                };
                var correctAnswersCount = assessmentSubmissionAnswers.Count(x => x.IsCorrect);
                AssessmentResult.TotalMark = assessment.Weightage * correctAnswersCount;
                await _unitOfWork
                    .GetRepository<AssessmentResult>()
                    .InsertAsync(AssessmentResult)
                    .ConfigureAwait(false);
            }

            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            var assessmentAchieve = await _unitOfWork
                .GetRepository<AssessmentSubmission>()
                .GetAll()
                .Where(
                    p =>
                        p.AssessmentId == assessment.Id
                        && p.UserId == currentUserId
                        && p.EndTime != default
                        && p.IsSubmissionError == false
                )
                .OrderByDescending(p => p.EndTime)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
            var assessmentQuestionCount = await _unitOfWork
                .GetRepository<AssessmentQuestion>()
                .CountAsync(x => x.AssessmentId == assessmentAchieve.AssessmentId);
            var assessmentResult = await _unitOfWork
                .GetRepository<AssessmentResult>()
                .GetFirstOrDefaultAsync(
                    predicate: x => x.AssessmentSubmissionId == assessmentAchieve.Id
                )
                .ConfigureAwait(false);
            var totalMarksObtained =
                assessmentResult.TotalMark * 100 / (assessment.Weightage * assessmentQuestionCount);
            var existingSkills = await _unitOfWork
                .GetRepository<SkillsCriteria>()
                .GetAllAsync(predicate: p => p.AssessmentId == assessmentAchieve.AssessmentId)
                .ConfigureAwait(false);
            var existingUserSkills = await _unitOfWork
                .GetRepository<UserSkills>()
                .GetAllAsync(predicate: p => p.UserId == currentUserId)
                .ConfigureAwait(false);

            foreach (var item in existingSkills)
            {
                // Check if the skill already exists for the user
                var skillExists = existingUserSkills.Any(us => us.SkillId == item.SkillId);

                if (
                    item.SkillAssessmentRule == SkillAssessmentRule.IsGreaterThan
                    && totalMarksObtained >= item.Percentage
                    && !skillExists // Check if the skill does not already exist for the user
                )
                {
                    var userSkill = new UserSkills
                    {
                        UserId = assessmentResult.UserId,
                        SkillId = (Guid)item.SkillId,
                        CreatedBy = currentUserId,
                        CreatedOn = DateTime.Now,
                    };

                    await _unitOfWork.GetRepository<UserSkills>().InsertAsync(userSkill); // Add the new userSkill to the context
                }
            }

            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

            return;
        }

        /// <summary>
        /// Handles to fetch result of a particular Assessment Id
        /// </summary>
        /// <param name="identity">the Assessment Id or Slug </param>
        /// <param name="currentUserId"></param>
        /// <returns>the instance of <see cref="AssessmentResultResponseModel"</returns>
        public async Task<SearchResult<AssessmentResultResponseModel>> GetResults(
            BaseSearchCriteria searchCriteria,
            string identity,
            Guid currentUserId
        )
        {
            try
            {
                var currentTimeStamp = DateTime.UtcNow;
                var assessment = await _unitOfWork
                    .GetRepository<Assessment>()
                    .GetFirstOrDefaultAsync(
                        predicate: p => p.Id.ToString() == identity || p.Slug == identity
                    )
                    .ConfigureAwait(false);

                if (assessment == null)
                {
                    _logger.LogWarning(
                        "Question set not found with identity: {identity}.",
                        identity
                    );
                    throw new EntityNotFoundException(_localizer.GetString("AssessmentNotFound"));
                }

                var isSuperAdminOrAdmin = await IsSuperAdminOrAdmin(currentUserId)
                    .ConfigureAwait(false);

                var predicate = PredicateBuilder.New<AssessmentResult>(true);
                predicate = predicate.And(p => p.AssessmentId == assessment.Id);

                if (assessment.CreatedBy != currentUserId && !isSuperAdminOrAdmin)
                {
                    predicate = predicate.And(p => p.UserId == currentUserId);
                }

                if (!string.IsNullOrWhiteSpace(searchCriteria.Search))
                {
                    var search = searchCriteria.Search.ToLower().Trim();
                    predicate = predicate.And(
                        x =>
                            x.User.LastName.ToLower().Trim().Contains(search)
                            || x.User.FirstName.ToLower().Trim().Contains(search)
                    );
                }

                var query = await _unitOfWork
                    .GetRepository<AssessmentResult>()
                    .GetAllAsync(predicate: predicate, include: src => src.Include(x => x.User))
                    .ConfigureAwait(false);

                var result = query
                    .GroupBy(x => x.UserId)
                    .Select(
                        x =>
                            new AssessmentResult
                            {
                                Id = x.FirstOrDefault(
                                    a => a.CreatedOn == x.Max(b => b.CreatedOn)
                                ).Id,
                                AssessmentId = assessment.Id,
                                UserId = x.FirstOrDefault(
                                    a => a.CreatedOn == x.Max(b => b.CreatedOn)
                                ).UserId,
                                TotalMark = x.FirstOrDefault(
                                    a => a.CreatedOn == x.Max(b => b.CreatedOn)
                                ).TotalMark,
                                NegativeMark = x.FirstOrDefault(
                                    a => a.CreatedOn == x.Max(b => b.CreatedOn)
                                ).NegativeMark,
                                User = x.FirstOrDefault(
                                    a => a.CreatedOn == x.Max(b => b.CreatedOn)
                                ).User,
                                CreatedOn = x.Max(a => a.CreatedOn),
                            }
                    )
                    .ToList();

                var paginatedResult = result.ToIPagedList(searchCriteria.Page, searchCriteria.Size);
                var response = new SearchResult<AssessmentResultResponseModel>
                {
                    Items = new List<AssessmentResultResponseModel>(),
                    CurrentPage = paginatedResult.CurrentPage,
                    PageSize = paginatedResult.PageSize,
                    TotalCount = paginatedResult.TotalCount,
                    TotalPage = paginatedResult.TotalPage,
                };
                paginatedResult.Items.ForEach(
                    res =>
                        response.Items.Add(
                            new AssessmentResultResponseModel
                            {
                                Id = res.Id,
                                AssessmentId = res.AssessmentId,
                                ObtainedMarks =
                                    (res.TotalMark - res.NegativeMark) > 0
                                        ? (res.TotalMark - res.NegativeMark)
                                        : 0,
                                User = new UserModel
                                {
                                    Id = (Guid)res.User?.Id,
                                    FullName = res.User?.FullName,
                                    Email = res.User?.Email,
                                    ImageUrl = res.User.ImageUrl,
                                }
                            }
                        )
                );
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while attempting to retrieving the results."
                );
                throw ex is ServiceException
                    ? ex
                    : new ServiceException(_localizer.GetString("ErrorAttemptingRetrievingResult"));
            }
        }

        /// <summary>
        /// Handles to fetch result of a particular student result
        /// </summary>
        /// <param name="identity">the question set id or slug </param>
        /// <param name="userId">the student user id </param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns>the instance of <see cref="StudentResultResponseModel"</returns>
        public async Task<StudentAssessmentResultResponseModel> GetStudentResult(
            string identity,
            Guid userId,
            Guid currentUserId
        )
        {
            try
            {
                var currentTimeStamp = DateTime.UtcNow;
                var assessment = await _unitOfWork
                    .GetRepository<Assessment>()
                    .GetFirstOrDefaultAsync(
                        predicate: p => p.Id.ToString() == identity || p.Slug == identity
                    )
                    .ConfigureAwait(false);

                if (assessment == null)
                {
                    _logger.LogWarning(
                        "Question set not found with identity: {identity}.",
                        identity
                    );
                    throw new EntityNotFoundException(_localizer.GetString("AssessmentNotFound"));
                }

                var isSuperAdminOrAdmin = await IsSuperAdminOrAdmin(currentUserId)
                    .ConfigureAwait(false);

                var predicate = PredicateBuilder.New<AssessmentSubmission>(true);
                predicate = predicate.And(p => p.AssessmentId == assessment.Id);
                predicate = predicate.And(p => p.UserId == userId);
                predicate = predicate.And(p => p.EndTime != default);
                predicate = predicate.And(
                    p => p.AssessmentSubmissionAnswers.Any(x => x.AssessmentSubmissionId == p.Id)
                );
                predicate = predicate.And(
                    p => p.AssessmentResults.Any(x => x.AssessmentSubmissionId == p.Id)
                );

                var questionSetSubmissions = await _unitOfWork
                    .GetRepository<AssessmentSubmission>()
                    .GetAllAsync(
                        predicate: predicate,
                        include: src => src.Include(x => x.User).Include(x => x.AssessmentResults)
                    )
                    .ConfigureAwait(false);

                if (questionSetSubmissions.Count == 0)
                {
                    return new StudentAssessmentResultResponseModel();
                }

                var user = questionSetSubmissions.FirstOrDefault()?.User;
                var response = new StudentAssessmentResultResponseModel
                {
                    AttemptCount = questionSetSubmissions.Count,
                    User = new UserModel
                    {
                        Id = (Guid)user?.Id,
                        FullName = user?.FullName,
                        Email = user?.Email,
                        ImageUrl = user?.ImageUrl,
                    },
                    AssessmentSetResultDetails = new List<AssessmentSetResultDetailModel>()
                };
                response.HasExceededAttempt = response.AttemptCount >= assessment.Retakes;
                response.EndDate = assessment.EndDate;
                questionSetSubmissions.ForEach(
                    res =>
                        response.AssessmentSetResultDetails.Add(
                            new AssessmentSetResultDetailModel
                            {
                                QuestionSetSubmissionId = res.Id,
                                SubmissionDate =
                                    res.EndTime != default
                                        ? res.EndTime
                                        : res.StartTime != default
                                            ? res.StartTime
                                            : res.CreatedOn,
                                TotalMarks =
                                    res.AssessmentResults.Count > 0
                                        ? res.AssessmentResults
                                            .FirstOrDefault()
                                            .TotalMark.ToString()
                                        : "",
                                NegativeMarks =
                                    res.AssessmentResults.Count > 0
                                        ? res.AssessmentResults
                                            .FirstOrDefault()
                                            .NegativeMark.ToString()
                                        : "",
                                ObtainedMarks =
                                    res.AssessmentResults.Count > 0
                                        ? (
                                            (
                                                res.AssessmentResults.FirstOrDefault().TotalMark
                                                - res.AssessmentResults
                                                    .FirstOrDefault()
                                                    .NegativeMark
                                            ) > 0
                                                ? (
                                                    res.AssessmentResults.FirstOrDefault().TotalMark
                                                    - res.AssessmentResults
                                                        .FirstOrDefault()
                                                        .NegativeMark
                                                )
                                                : 0
                                        ).ToString()
                                        : "",
                                Duration =
                                    assessment.Duration != 0
                                        ? TimeSpan
                                            .FromSeconds(assessment.Duration)
                                            .ToString(@"hh\:mm\:ss")
                                        : string.Empty,
                                CompleteDuration =
                                    assessment.Duration == 0 || res.EndTime == default
                                        ? string.Empty
                                        : TimeSpan
                                            .FromSeconds(
                                                (
                                                    Convert.ToDateTime(res.StartTime)
                                                    - Convert.ToDateTime(res.EndTime)
                                                ).TotalSeconds
                                            )
                                            .ToString(@"hh\:mm\:ss"),
                            }
                        )
                );
                response.AssessmentSetResultDetails = response.AssessmentSetResultDetails
                    .OrderByDescending(x => x.SubmissionDate)
                    .ToList();
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while attempting to retrieving the student result."
                );
                throw ex is ServiceException
                    ? ex
                    : new ServiceException(_localizer.GetString("ErrorRetrievingStudentResult"));
            }
        }

        /// <summary>
        /// Get result detail of question set submission
        /// </summary>
        /// <param name="identity">the question set id or slug</param>
        /// <param name="questionSetSubmissionId">the question set submission id</param>
        /// <param name="currentUserId">the current user id</param>
        /// <returns>the instance of <see cref="AssessmentUserResultResponseModel"</returns>
        public async Task<AssessmentUserResultResponseModel> GetResultDetail(
            string identity,
            Guid assessmentSubmissionId,
            Guid currentUserId
        )
        {
            try
            {
                var currentTimeStamp = DateTime.UtcNow;
                var assessment = await _unitOfWork
                    .GetRepository<Assessment>()
                    .GetFirstOrDefaultAsync(
                        predicate: p => p.Id.ToString() == identity || p.Slug == identity
                    )
                    .ConfigureAwait(false);

                if (assessment == null)
                {
                    _logger.LogWarning(
                        "Question set not found with identity: {identity}.",
                        identity
                    );
                    throw new EntityNotFoundException(_localizer.GetString("QuestionSetNotFound"));
                }

                var isSuperAdminOrAdmin = await IsSuperAdminOrAdmin(currentUserId)
                    .ConfigureAwait(false);

                var predicate = PredicateBuilder.New<AssessmentResult>(true);
                predicate = predicate.And(
                    p =>
                        p.AssessmentId == assessment.Id
                        && p.AssessmentSubmissionId == assessmentSubmissionId
                );
                var assessmentResult = await _unitOfWork
                    .GetRepository<AssessmentResult>()
                    .GetFirstOrDefaultAsync(
                        predicate: predicate,
                        include: src => src.Include(x => x.User)
                    )
                    .ConfigureAwait(false);
                if (assessmentResult == null)
                {
                    _logger.LogWarning(
                        "Assessment result not found for user with id: {currentUserId} and question-set-id: {questionSetId}.",
                        currentUserId,
                        assessment.Id
                    );
                    throw new EntityNotFoundException(
                        _localizer.GetString("AssessmentResultNotFound")
                    );
                }

                var assessmentSubmission = await _unitOfWork
                    .GetRepository<AssessmentSubmission>()
                    .GetFirstOrDefaultAsync(
                        predicate: p =>
                            p.Id == assessmentSubmissionId && p.AssessmentId == assessment.Id
                    )
                    .ConfigureAwait(false);
                if (assessmentSubmission == null)
                {
                    _logger.LogWarning(
                        "Question set submission not found with id: {questionSetSubmissionId} for user id: {currentUserId}.",
                        assessmentSubmission,
                        currentUserId
                    );
                    throw new EntityNotFoundException(
                        _localizer.GetString("ExamSubmissionNotFound")
                    );
                }

                var assessmentSubmissionAnswers = await _unitOfWork
                    .GetRepository<AssessmentSubmissionAnswer>()
                    .GetAllAsync(
                        predicate: p => p.AssessmentSubmissionId == assessmentSubmissionId,
                        include: src => src.Include(x => x.AssessmentQuestion.AssessmentOptions)
                    )
                    .ConfigureAwait(false);

                var ObtainedMarks = assessmentResult.TotalMark - assessmentResult.NegativeMark;
                var resultMark = ObtainedMarks > 0 ? ObtainedMarks : 0;
                var responseModel = new AssessmentUserResultResponseModel()
                {
                    QuestionSetSubmissionId = assessmentSubmissionId,
                    Name = assessment.Title,
                    Description = assessment.Description,
                    TotalMarks = assessment.Weightage * assessmentSubmissionAnswers.Count,
                    NegativeMarks = assessmentResult.NegativeMark,
                    ObtainedMarks = resultMark,
                    SubmissionDate =
                        assessmentSubmission?.EndTime
                        ?? assessmentSubmission?.StartTime
                        ?? assessmentSubmission.CreatedOn,
                    Duration =
                        assessment.Duration != 0
                            ? TimeSpan.FromSeconds(assessment.Duration).ToString(@"hh\:mm\:ss")
                            : string.Empty,
                    CompleteDuration =
                        assessment.Duration == 0 || assessmentSubmission.EndTime == default
                            ? string.Empty
                            : TimeSpan
                                .FromSeconds(
                                    (
                                        Convert.ToDateTime(assessmentSubmission?.EndTime)
                                        - Convert.ToDateTime(assessmentSubmission?.StartTime)
                                    ).TotalSeconds
                                )
                                .ToString(@"hh\:mm\:ss"),
                    User = new UserModel
                    {
                        Id = (Guid)(assessmentResult?.User?.Id),
                        Email = assessmentResult?.User?.Email,
                        FullName = assessmentResult?.User?.FullName,
                        ImageUrl = assessmentResult?.User?.ImageUrl,
                    },
                    Results = new List<AssessmentAnswerResultModel>()
                };

                foreach (var item in assessmentSubmissionAnswers)
                {
                    var result = new AssessmentAnswerResultModel
                    {
                        Id = item.AssessmentQuestion.Id,
                        Name = item.AssessmentQuestion?.Name,
                        Hints = item.AssessmentQuestion?.Hints,
                        Description = item.AssessmentQuestion?.Description,
                        Type = item.AssessmentQuestion.Type,
                        IsCorrect = item.IsCorrect,
                        QuestionOptions = new List<AssessmentQuestionResultOption>(),
                        OrderNumber = item.AssessmentQuestion.Order
                    };

                    var selectedAnsIds = !string.IsNullOrWhiteSpace(item.SelectedAnswers)
                        ? item.SelectedAnswers.Split(",").Select(Guid.Parse).ToList()
                        : new List<Guid>();
                    item.AssessmentQuestion.AssessmentOptions
                        .OrderBy(o => o.Order)
                        .ForEach(
                            opt =>
                                result.QuestionOptions.Add(
                                    new AssessmentQuestionResultOption
                                    {
                                        Id = opt.Id,
                                        Value = opt.Option,
                                        IsCorrect = opt.IsCorrect,
                                        IsSelected = selectedAnsIds.Contains(opt.Id)
                                    }
                                )
                        );

                    responseModel.Results.Add(result);
                }

                responseModel.Results = responseModel.Results.OrderBy(x => x.OrderNumber).ToList();
                return responseModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting result details.");
                throw ex is ServiceException
                    ? ex
                    : new ServiceException(_localizer.GetString("ErrorGettingResult"));
            }
        }

        public async Task<IList<AssessmentResultExportModel>> GetResultsExportAsync(
            string identity,
            Guid currentUserId
        )
        {
            var assessment = await _unitOfWork
                    .GetRepository<Assessment>()
                    .GetFirstOrDefaultAsync(
                        predicate: p => p.Id.ToString() == identity || p.Slug == identity
                    )
                    .ConfigureAwait(false);

                var isSuperAdminOrAdmin = await IsSuperAdminOrAdmin(currentUserId)
                    .ConfigureAwait(false);

                var predicate = PredicateBuilder.New<AssessmentResult>(true);
                predicate = predicate.And(p => p.AssessmentId == assessment.Id);

                if (assessment.CreatedBy != currentUserId && !isSuperAdminOrAdmin)
                {
                    predicate = predicate.And(p => p.UserId == currentUserId);
                }

                var query = await _unitOfWork
                    .GetRepository<AssessmentResult>()
                    .GetAllAsync(predicate: predicate, include: src => src.Include(x => x.User))
                    .ConfigureAwait(false);

                var result = query
                    .GroupBy(x => x.UserId)
                    .Select(
                        x =>
                            new AssessmentResult
                            {
                                Id = x.FirstOrDefault(
                                    a => a.CreatedOn == x.Max(b => b.CreatedOn)
                                ).Id,
                                AssessmentId = assessment.Id,
                                UserId = x.FirstOrDefault(
                                    a => a.CreatedOn == x.Max(b => b.CreatedOn)
                                ).UserId,
                                TotalMark = x.FirstOrDefault(
                                    a => a.CreatedOn == x.Max(b => b.CreatedOn)
                                ).TotalMark,
                                NegativeMark = x.FirstOrDefault(
                                    a => a.CreatedOn == x.Max(b => b.CreatedOn)
                                ).NegativeMark,
                                User = x.FirstOrDefault(
                                    a => a.CreatedOn == x.Max(b => b.CreatedOn)
                                ).User,
                                CreatedOn = x.Max(a => a.CreatedOn),
                            }
                    )
                    .ToList();
                var response = new List<AssessmentResultExportModel>();
                result.ForEach(
                    res =>response.Add(
                            new AssessmentResultExportModel
                            {
                                TotalMarks =
                                    (res.TotalMark - res.NegativeMark) > 0
                                        ? (res.TotalMark - res.NegativeMark)
                                        : 0,
                                StudentName=res.User?.FullName
                            }
                        )
                        
                );
                return response;
        }
    }
}

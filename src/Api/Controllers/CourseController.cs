namespace Lingtren.Api.Controllers
{
    using System.Globalization;
    using CsvHelper;
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

    public class CourseController : BaseApiController
    {
        private readonly ICourseService courseService;
        private readonly IValidator<CourseRequestModel> validator;
        private readonly IValidator<CourseStatusRequestModel> courseStatusValidator;
        private readonly IStringLocalizer<ExceptionLocalizer> localizer;

        public CourseController(
            ICourseService courseService,
            IValidator<CourseRequestModel> validator,
            IValidator<CourseStatusRequestModel> courseStatusValidator,
            IStringLocalizer<ExceptionLocalizer> localizer
        )
        {
            this.courseService = courseService;
            this.validator = validator;
            this.courseStatusValidator = courseStatusValidator;
            this.localizer = localizer;
        }

        /// <summary>
        /// course search api.
        /// </summary>
        /// <returns> the list of <see cref="CourseResponseModel" /> .</returns>
        [HttpGet]
        public async Task<SearchResult<CourseResponseModel>> SearchAsync(
            [FromQuery] CourseBaseSearchCriteria searchCriteria
        )
        {
            if (searchCriteria.UserId != Guid.Empty)
            {
                searchCriteria.CurrentUserId = searchCriteria.UserId;
            }
            else
            {
                searchCriteria.CurrentUserId = CurrentUser.Id;
            }

            var searchResult = await courseService
                .SearchAsync(searchCriteria, true)
                .ConfigureAwait(false);

            var response = new SearchResult<CourseResponseModel>
            {
                Items = new List<CourseResponseModel>(),
                CurrentPage = searchResult.CurrentPage,
                PageSize = searchResult.PageSize,
                TotalCount = searchResult.TotalCount,
                TotalPage = searchResult.TotalPage,
            };

            searchResult.Items.ForEach(p =>
                response.Items.Add(
                    new CourseResponseModel(
                        p,
                        searchCriteria.EnrollmentStatus == null
                            ? courseService.GetUserCourseEnrollmentStatus(
                                p,
                                searchCriteria.CurrentUserId
                            )
                            : searchCriteria.EnrollmentStatus.FirstOrDefault(),
                        courseService.GetUserEligibilityStatus(p, searchCriteria.CurrentUserId)
                    )
                )
            );
            return response;
        }

        /// <summary>
        /// create course api.
        /// </summary>
        /// <param name="model"> the instance of <see cref="CourseRequestModel" />. </param>
        /// <returns> the instance of <see cref="CourseResponseModel" /> .</returns>
        [HttpPost]
        public async Task<CourseResponseModel> CreateAsync(CourseRequestModel model)
        {
            await courseService.ISSuperAdminAdminOrTrainerAsync(CurrentUser.Id);
            await validator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            var currentTimeStamp = DateTime.UtcNow;
            var entity = new Course
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Language = model.Language,
                GroupId = model.GroupId,
                LevelId = model.LevelId,
                Duration = model.Duration,
                Description = model.Description,
                ThumbnailUrl = model.ThumbnailUrl,
                Status = CourseStatus.Draft,
                CreatedOn = currentTimeStamp,
                CreatedBy = CurrentUser.Id,
                UpdatedOn = currentTimeStamp,
                UpdatedBy = CurrentUser.Id,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                IsUnlimitedEndDate = model.IsUnlimitedEndDate,
                CourseTags = new List<CourseTag>(),
                CourseTeachers = new List<CourseTeacher>(),
                TrainingEligibilities = new List<TrainingEligibility>(),
            };
            foreach (var tagId in model.TagIds)
            {
                entity.CourseTags.Add(
                    new CourseTag
                    {
                        Id = Guid.NewGuid(),
                        TagId = tagId,
                        CourseId = entity.Id,
                        CreatedOn = currentTimeStamp,
                        CreatedBy = CurrentUser.Id,
                        UpdatedOn = currentTimeStamp,
                        UpdatedBy = CurrentUser.Id,
                    }
                );
            }

            entity.CourseTeachers.Add(
                new CourseTeacher
                {
                    Id = Guid.NewGuid(),
                    CourseId = entity.Id,
                    UserId = CurrentUser.Id,
                    CreatedOn = currentTimeStamp,
                    CreatedBy = CurrentUser.Id,
                    UpdatedOn = currentTimeStamp,
                    UpdatedBy = CurrentUser.Id,
                }
            );

            foreach (var criteria in model.TrainingEligibilities)
            {
                if (criteria.Eligibility != 0)
                {
                    entity.TrainingEligibilities.Add(
                        new TrainingEligibility
                        {
                            Id = Guid.NewGuid(),
                            CourseId = entity.Id,
                            EligibilityId = criteria.EligibilityId,
                            TrainingEligibilityEnum = criteria.Eligibility,
                            CreatedBy = CurrentUser.Id,
                            CreatedOn = currentTimeStamp
                        }
                    );
                }
            }

            var response = await courseService.CreateAsync(entity).ConfigureAwait(false);
            return new CourseResponseModel(response, null);
        }

        /// <summary>
        /// get course by id or slug.
        /// </summary>
        /// <param name="identity"> the course id or slug.</param>
        /// <returns> the instance of <see cref="CourseResponseModel" /> .</returns>
        [HttpGet("{identity}")]
        public async Task<CourseResponseModel> Get(string identity)
        {
            return await courseService
                .GetDetailAsync(identity, CurrentUser.Id)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// update course api.
        /// </summary>
        /// <param name="identity"> id or slug. </param>
        /// <param name="model"> the instance of <see cref="CourseRequestModel" />. </param>
        /// <returns> the instance of <see cref="CourseResponseModel" /> .</returns>
        [HttpPut("{identity}")]
        public async Task<CourseResponseModel> UpdateAsync(
            string identity,
            CourseRequestModel model
        )
        {
            await validator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            var savedEntity = await courseService
                .UpdateAsync(identity, model, CurrentUser.Id)
                .ConfigureAwait(false);
            var userStatus = courseService.GetUserCourseEnrollmentStatus(
                savedEntity,
                CurrentUser.Id
            );
            return new CourseResponseModel(savedEntity, userStatus);
        }

        /// <summary>
        /// delete course api.
        /// </summary>
        /// <param name="identity"> id or slug. </param>
        /// <returns> the task complete. </returns>
        [HttpDelete("{identity}")]
        public async Task<IActionResult> DeleteAsync(string identity)
        {
            await courseService.DeleteCourseAsync(identity, CurrentUser.Id).ConfigureAwait(false);
            return Ok(
                new CommonResponseModel()
                {
                    Success = true,
                    Message = localizer.GetString("TrainingRemoved")
                }
            );
        }

        /// <summary>
        /// change course status api.
        /// </summary>
        /// <param name="identity"> id or slug. </param>
        /// <returns> the task complete. </returns>
        [HttpPatch("{identity}/updateCourse")]
        public async Task<IActionResult> UpdateCourse(string identity)
        {
            await courseService
                .UpdateCourseStatusAsync(identity, CurrentUser.Id)
                .ConfigureAwait(false);
            return Ok(
                new CommonResponseModel()
                {
                    Success = true,
                    Message = localizer.GetString("TrainingUpdated")
                }
            );
        }

        /// <summary>
        /// change course status api.
        /// </summary>
        /// <param name="model"> the instance of <see cref="CourseStatusRequestModel" /> . </param>
        /// <returns> the task complete. </returns>
        [HttpPatch("status")]
        public async Task<IActionResult> ChangeStatus(CourseStatusRequestModel model)
        {
            await courseStatusValidator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            var result = await courseService
                .ChangeStatusAsync(model, CurrentUser.Id)
                .ConfigureAwait(false);
            return Ok(new CommonResponseModel() { Success = true, Message = result });
        }

        /// <summary>
        /// enroll course api.
        /// </summary>
        /// <param name="identity"> the course id or slug.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        [HttpPost("{identity}/enroll")]
        public async Task<IActionResult> Enroll(string identity)
        {
            await courseService.EnrollmentAsync(identity, CurrentUser.Id).ConfigureAwait(false);
            return Ok(
                new CommonResponseModel()
                {
                    Success = true,
                    Message = localizer.GetString("UserEnrolled")
                }
            );
        }

        /// <summary>
        /// Course Lesson statistics api.
        /// </summary>
        /// <param name="identity"> the course id or slug.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        [HttpGet("{identity}/lessonStatistics")]
        public async Task<SearchResult<LessonStatisticsResponseModel>> LessonStatistics(
            string identity,
            [FromQuery] BaseSearchCriteria criteria
        ) =>
            await courseService
                .LessonStatistics(identity, CurrentUser.Id, criteria)
                .ConfigureAwait(false);

        /// <summary>
        /// get course statistics api.
        /// </summary>
        /// <param name="identity"> the course id or slug. </param>
        /// <returns> the instance of <see cref="CourseStatisticsResponseModel" /> >.</returns>
        [HttpGet("{identity}/statistics")]
        public async Task<CourseStatisticsResponseModel> Statistics(string identity) =>
            await courseService
                .GetCourseStatisticsAsync(identity, CurrentUser.Id)
                .ConfigureAwait(false);

        /// <summary>
        /// Course Lesson Student's report api.
        /// </summary>
        /// <param name="identity"> the course id or slug.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        [HttpGet("{identity}/lessonStatistics/{lessonIdentity}")]
        public async Task<SearchResult<LessonStudentResponseModel>> LessonDetailStatistics(
            string identity,
            string lessonIdentity,
            [FromQuery] BaseSearchCriteria searchCriteria
        )
        {
            searchCriteria.CurrentUserId = CurrentUser.Id;
            return await courseService
                .LessonStudentsReport(identity, lessonIdentity, searchCriteria)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Course Lesson Student's report api.
        /// </summary>
        /// <param name="identity"> the course id or slug.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        [HttpGet("{identity}/lessonStatistics/{lessonIdentity}/summary")]
        public async Task<ExamSummaryResponseModel> LessonDetailSummary(
            string identity,
            string lessonIdentity,
            [FromQuery] BaseSearchCriteria searchCriteria
        )
        {
            searchCriteria.CurrentUserId = CurrentUser.Id;
            return await courseService
                .ExamSummaryReport(identity, lessonIdentity, searchCriteria)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Course Lesson Student's report api.
        /// </summary>
        /// <param name="identity"> the course id or slug.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        [HttpGet("{identity}/lessonStatistics/{lessonIdentity}/submission")]
        public async Task<IList<ExamSubmissionResponseModel>> LessonDetailSubmission(
            string identity,
            string lessonIdentity,
            [FromQuery] BaseSearchCriteria searchCriteria
        )
        {
            searchCriteria.CurrentUserId = CurrentUser.Id;
            return await courseService
                .ExamSubmissionReport(identity, lessonIdentity, searchCriteria)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Course Lesson Student's assignment summary report api.
        /// </summary>
        /// <param name="identity"> the course id or slug.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        [HttpGet("{identity}/lessonStatistics/{lessonIdentity}/AssignmentSummary")]
        public async Task<AssignmentSummaryResponseModel> AssignmentDetailSummary(
            string identity,
            string lessonIdentity,
            [FromQuery] BaseSearchCriteria searchCriteria
        )
        {
            searchCriteria.CurrentUserId = CurrentUser.Id;
            return await courseService
                .AssignmentStudentsReport(identity, lessonIdentity, searchCriteria)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Course Lesson Student's assignment summary report api.
        /// </summary>
        /// <param name="identity"> the course id or slug.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        [HttpGet("{identity}/lessonStatistics/{lessonIdentity}/AssignmentSubmission")]
        public async Task<
            SearchResult<AssignmentSubmissionResponseModel>
        > AssignmentDetailSubmission(
            string identity,
            string lessonIdentity,
            [FromQuery] BaseSearchCriteria searchCriteria
        )
        {
            searchCriteria.CurrentUserId = CurrentUser.Id;
            return await courseService
                .AssignmentSubmissionStudentsReport(identity, lessonIdentity, searchCriteria)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Course student statistics api.
        /// </summary>
        /// <param name="identity"> the course id or slug.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        [HttpGet("{identity}/studentStatistics")]
        public async Task<SearchResult<StudentCourseStatisticsResponseModel>> StudentStatistics(
            string identity,
            [FromQuery] BaseSearchCriteria searchCriteria
        )
        {
            return await courseService
                .StudentStatistics(identity, CurrentUser.Id, searchCriteria)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Course student statistics api.
        /// </summary>
        /// <param name="identity"> the course id or slug.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        [HttpGet("{identity}/studentStatistics/{userId}")]
        public async Task<IList<LessonStudentResponseModel>> StudentStatistics(
            string identity,
            Guid userId
        )
        {
            return await courseService
                .StudentLessonsDetail(identity, userId, CurrentUser.Id)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// User course api.
        /// </summary>
        /// <param name="userId">the requested user id.</param>
        /// <param name="searchCriteria">the instance of <see cref="BaseSearchCriteria"/>.</param>
        /// <returns>the search result of <see cref="CourseResponseModel"/>.</returns>
        [HttpGet("user/{userId}")]
        public async Task<SearchResult<CourseResponseModel>> GetUserCourses(
            Guid userId,
            [FromQuery] BaseSearchCriteria searchCriteria
        )
        {
            searchCriteria.CurrentUserId = CurrentUser.Id;
            return await courseService.GetUserCourses(userId, searchCriteria).ConfigureAwait(false);
        }

        [HttpGet("{identity}/lessonStatistics/{lessonIdentity}/export")]
        public async Task<IActionResult> Export(string lessonIdentity)
        {
            var response = await courseService
                .GetResultsExportAsync(lessonIdentity, CurrentUser.Id)
                .ConfigureAwait(false);

            using var memoryStream = new MemoryStream();
            using var steamWriter = new StreamWriter(memoryStream);
            using (var csv = new CsvWriter(steamWriter, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(response);
                csv.Flush();
            }

            return File(memoryStream.ToArray(), "text/csv", "Results.csv");
        }

        /// <summary>
        /// User course api.
        /// </summary>
        /// <param name="userId">the requested user id.</param>
        /// <param name="lessonId">the instance of <see cref="BaseSearchCriteria"/>.</param>
        /// <returns>the search result of <see cref="CourseResponseModel"/>.</returns>
        [HttpGet("{identity}/lessonStatistics/{lessonIdentity}/exportSubmission")]
        public async Task<IActionResult> ExportSubmission(string lessonIdentity)
        {
            var response = await courseService
                .GetResultsExportAsync(lessonIdentity, CurrentUser.Id)
                .ConfigureAwait(false);

            using var memoryStream = new MemoryStream();
            using var steamWriter = new StreamWriter(memoryStream);
            using (var csv = new CsvWriter(steamWriter, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(response);
                csv.Flush();
            }

            return File(memoryStream.ToArray(), "text/csv", "Results.csv");
        }
    }
}

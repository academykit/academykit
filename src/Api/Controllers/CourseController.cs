namespace Lingtren.Api.Controllers
{
    using Application.Common.Models.RequestModels;
    using FluentValidation;
    using Lingtren.Api.Common;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using Lingtren.Infrastructure.Localization;
    using Lingtren.Infrastructure.Services;
    using LinqKit;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    public class CourseController : BaseApiController
    {
        private readonly ICourseService _courseService;
        private readonly IValidator<CourseRequestModel> _validator;
        private readonly IValidator<CourseStatusRequestModel> _courseStatusValidator;
        private readonly ILogger<CourseController> _logger;
        private readonly IStringLocalizer<ExceptionLocalizer> _localizer;

        public CourseController(
            ICourseService courseService,
            IValidator<CourseRequestModel> validator,
            ILogger<CourseController> logger,
            IValidator<CourseStatusRequestModel> courseStatusValidator,
            IStringLocalizer<ExceptionLocalizer> localizer)
        {
            _courseService = courseService;
            _validator = validator;
            _courseStatusValidator = courseStatusValidator;
            _logger = logger;
            _localizer = localizer;
        }

        /// <summary>
        /// course search api
        /// </summary>
        /// <returns> the list of <see cref="CourseResponseModel" /> .</returns>
        [HttpGet]
        public async Task<SearchResult<CourseResponseModel>> SearchAsync([FromQuery] CourseBaseSearchCriteria searchCriteria)
        {
            if (searchCriteria.UserId != Guid.Empty)
            {
                searchCriteria.CurrentUserId = searchCriteria.UserId;
            }
            else
            {
                searchCriteria.CurrentUserId = CurrentUser.Id;
            }
            var searchResult = await _courseService.SearchAsync(searchCriteria).ConfigureAwait(false);

            var response = new SearchResult<CourseResponseModel>
            {
                Items = new List<CourseResponseModel>(),
                CurrentPage = searchResult.CurrentPage,
                PageSize = searchResult.PageSize,
                TotalCount = searchResult.TotalCount,
                TotalPage = searchResult.TotalPage,
            };

            searchResult.Items.ForEach(p =>
                 response.Items.Add(new CourseResponseModel(p, searchCriteria.EnrollmentStatus == null ? _courseService.GetUserCourseEnrollmentStatus(p,searchCriteria.CurrentUserId) : searchCriteria.EnrollmentStatus.FirstOrDefault()))
             );
            return response;
        }

        /// <summary>
        /// create course api
        /// </summary>
        /// <param name="model"> the instance of <see cref="CourseRequestModel" />. </param>
        /// <returns> the instance of <see cref="CourseResponseModel" /> .</returns>
        [HttpPost]
        public async Task<CourseResponseModel> CreateAsync(CourseRequestModel model)
        {
            await _courseService.ISSuperAdminAdminOrTrainerAsync(CurrentUser.Id);
            await _validator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
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
                CourseTags = new List<CourseTag>(),
                CourseTeachers = new List<CourseTeacher>()
            };
            foreach (var tagId in model.TagIds)
            {
                entity.CourseTags.Add(new CourseTag
                {
                    Id = Guid.NewGuid(),
                    TagId = tagId,
                    CourseId = entity.Id,
                    CreatedOn = currentTimeStamp,
                    CreatedBy = CurrentUser.Id,
                    UpdatedOn = currentTimeStamp,
                    UpdatedBy = CurrentUser.Id,
                });
            }
            entity.CourseTeachers.Add(new CourseTeacher
            {
                Id = Guid.NewGuid(),
                CourseId = entity.Id,
                UserId = CurrentUser.Id,
                CreatedOn = currentTimeStamp,
                CreatedBy = CurrentUser.Id,
                UpdatedOn = currentTimeStamp,
                UpdatedBy = CurrentUser.Id,
            });

            var response = await _courseService.CreateAsync(entity).ConfigureAwait(false);
            return new CourseResponseModel(response, null);
        }

        /// <summary>
        /// get course by id or slug
        /// </summary>
        /// <param name="identity"> the course id or slug</param>
        /// <returns> the instance of <see cref="CourseResponseModel" /> .</returns>
        [HttpGet("{identity}")]
        public async Task<CourseResponseModel> Get(string identity)
        {
            return await _courseService.GetDetailAsync(identity, CurrentUser.Id).ConfigureAwait(false);
        }

        /// <summary>
        /// update course api
        /// </summary>
        /// <param name="identity"> id or slug </param>
        /// <param name="model"> the instance of <see cref="CourseRequestModel" />. </param>
        /// <returns> the instance of <see cref="CourseResponseModel" /> .</returns>
        [HttpPut("{identity}")]
        public async Task<CourseResponseModel> UpdateAsync(string identity, CourseRequestModel model)
        {
            await _validator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var savedEntity = await _courseService.UpdateAsync(identity, model, CurrentUser.Id).ConfigureAwait(false);
            var userStatus = _courseService.GetUserCourseEnrollmentStatus(savedEntity, CurrentUser.Id);
            return new CourseResponseModel(savedEntity, userStatus);
        }

        /// <summary>
        /// delete course api
        /// </summary>
        /// <param name="identity"> id or slug </param>
        /// <returns> the task complete </returns>
        [HttpDelete("{identity}")]
        public async Task<IActionResult> DeleteAsync(string identity)
        {
            await _courseService.DeleteCourseAsync(identity, CurrentUser.Id).ConfigureAwait(false);
            return Ok(new CommonResponseModel() { Success = true, Message = _localizer.GetString("TrainingRemoved") });
        }

        /// <summary>
        /// change course status api
        /// </summary>
        /// <param name="identity"> id or slug </param>
        /// <returns> the task complete </returns>
        [HttpPatch("{identity}/updateCourse")]
        public async Task<IActionResult> UpdateCourse(string identity)
        {
            await _courseService.UpdateCourseStatusAsync(identity, CurrentUser.Id).ConfigureAwait(false);
            return Ok(new CommonResponseModel() { Success = true, Message = _localizer.GetString("TrainingUpdated") });
        }

        /// <summary>
        /// change course status api
        /// </summary>
        /// <param name="model"> the instance of <see cref="CourseStatusRequestModel" /> . </param>
        /// <returns> the task complete </returns>
        [HttpPatch("status")]
        public async Task<IActionResult> ChangeStatus(CourseStatusRequestModel model)
        {
            await _courseStatusValidator.ValidateAsync(model,options => options.ThrowOnFailures()).ConfigureAwait(false);
            await _courseService.ChangeStatusAsync(model, CurrentUser.Id).ConfigureAwait(false);
            return Ok(new CommonResponseModel() { Success = true, Message = _localizer.GetString("TrainingStatus") });
        }

        /// <summary>
        /// enroll course api
        /// </summary>
        /// <param name="identity"> the course id or slug.</param>
        [HttpPost("{identity}/enroll")]
        public async Task<IActionResult> Enroll(string identity)
        {
            await _courseService.EnrollmentAsync(identity, CurrentUser.Id).ConfigureAwait(false);
            return Ok(new CommonResponseModel() { Success = true, Message = _localizer.GetString("UserEnrolled") });
        }

        /// <summary>
        /// Course Lesson statistics api
        /// </summary>
        /// <param name="identity"> the course id or slug.</param>
        [HttpGet("{identity}/lessonStatistics")]
        public async Task<SearchResult<LessonStatisticsResponseModel>> LessonStatistics(string identity,[FromQuery] BaseSearchCriteria criteria) => await _courseService.LessonStatistics(identity, CurrentUser.Id,criteria).ConfigureAwait(false);

        /// <summary>
        /// get course statistics api
        /// </summary>
        /// <param name="identity"> the course id or slug </param>
        /// <returns> the instance of <see cref="CourseStatisticsResponseModel" /> ></returns>
        [HttpGet("{identity}/statistics")]
        public async Task<CourseStatisticsResponseModel> Statistics(string identity) => await _courseService.GetCourseStatisticsAsync(identity, CurrentUser.Id).ConfigureAwait(false);


        /// <summary>
        /// Course Lesson Student's report api
        /// </summary>
        /// <param name="identity"> the course id or slug.</param>
        [HttpGet("{identity}/lessonStatistics/{lessonIdentity}")]
        public async Task<SearchResult<LessonStudentResponseModel>> LessonDetailStatistics(string identity, string lessonIdentity,[FromQuery] BaseSearchCriteria searchCriteria)
        {
            searchCriteria.CurrentUserId = CurrentUser.Id;
            return await _courseService.LessonStudentsReport(identity,lessonIdentity, searchCriteria).ConfigureAwait(false);
        }

        /// <summary>
        /// Course student statistics api
        /// </summary>
        /// <param name="identity"> the course id or slug.</param>
        [HttpGet("{identity}/studentStatistics")]
        public async Task<SearchResult<StudentCourseStatisticsResponseModel>> StudentStatistics(string identity, [FromQuery] BaseSearchCriteria searchCriteria)
        {
            return await _courseService.StudentStatistics(identity, CurrentUser.Id, searchCriteria).ConfigureAwait(false);
        }

        /// <summary>
        /// Course student statistics api
        /// </summary>
        /// <param name="identity"> the course id or slug.</param>
        [HttpGet("{identity}/studentStatistics/{userId}")]
        public async Task<IList<LessonStudentResponseModel>> StudentStatistics(string identity, Guid userId)
        {
            return await _courseService.StudentLessonsDetail(identity, userId, CurrentUser.Id).ConfigureAwait(false);
        }

        /// <summary>
        /// User course api
        /// </summary>
        /// <param name="userId">the requested user id</param>
        /// <param name="searchCriteria">the instance of <see cref="BaseSearchCriteria"/></param>
        /// <returns>the search result of <see cref="CourseResponseModel"/></returns>
        [HttpGet("user/{userId}")]
        public async Task<SearchResult<CourseResponseModel>> GetUserCourses(Guid userId, [FromQuery] BaseSearchCriteria searchCriteria)
        {
            searchCriteria.CurrentUserId = CurrentUser.Id;
            return await _courseService.GetUserCourses(userId, searchCriteria).ConfigureAwait(false);
        }
    }
}

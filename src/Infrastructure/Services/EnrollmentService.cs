using Lingtren.Application.Common.Dtos;
using Lingtren.Application.Common.Exceptions;
using Lingtren.Application.Common.Interfaces;
using Lingtren.Application.Common.Models.ResponseModels;
using Lingtren.Domain.Entities;
using Lingtren.Domain.Enums;
using Lingtren.Infrastructure.Common;
using Lingtren.Infrastructure.Helpers;
using Lingtren.Infrastructure.Localization;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Lingtren.Infrastructure.Services
{
    public class EnrollmentService : BaseGenericService<User, EnrollmentBaseSearchCritera>, IEnrollmentService
    {
        public EnrollmentService(IUnitOfWork unitOfWork, ILogger<EnrollmentService> logger, IStringLocalizer<ExceptionLocalizer> localizer) : base(unitOfWork, logger, localizer)
        {
        }

        /// <summary>
        /// enroll user in training
        /// </summary>
        /// <param name="email">user email or mobile number</param>
        /// <param name="currentUserId">current user id</param>
        /// <param name="courseIdentity">course id or slug</param>
        /// <returns>Task completed</returns>
        /// <exception cref="EntityNotFoundException"></exception>
        public async Task<string> EnrollUserAsync(IList<string> emails, Guid currentUserId, string courseIdentity)
        {
            return await ExecuteWithResultAsync(async () =>
            {
                var course = await _unitOfWork.GetRepository<Course>().GetFirstOrDefaultAsync(predicate: p => p.Id.ToString() == courseIdentity || p.Slug == courseIdentity).ConfigureAwait(false);
                if (course == default)
                {
                    throw new EntityNotFoundException(_localizer.GetString("CourseNotFound"));
                }
                foreach (var email in emails)
                {
                    var isMatch = await CommonHelper.ValidateEmailFormat(email.Trim());
                    if (!isMatch)
                    {
                        throw new ForbiddenException($"{_localizer.GetString("InvalidEmailFormat")}" + " " + email);
                    }
                }
                var hasAUthority = await IsSuperAdminOrAdminOrTrainerOfTraining(currentUserId, course.Id.ToString(), TrainingTypeEnum.Course);
                var userEmail = emails.Select(email => email.ToLower().Trim()).ToList();
                var message = new StringBuilder();
                var userList = await _unitOfWork.GetRepository<User>().GetAllAsync(predicate: p => emails.Contains(p.Email) || emails.Contains(p.MobileNumber)).ConfigureAwait(false);
                if (userList.Count == default)
                {
                    throw new EntityNotFoundException(_localizer.GetString("UserNotFound"));
                }
                var validUserIds = userList.Select(x => x.Id).ToList();
                var validEmail = userList.Select(x => x.Email).ToList();
                var invalidusers = emails.Except(validEmail).ToList();
                if (invalidusers.Count != default)
                {
                    message.AppendLine($"{_localizer.GetString("UserNotFoundWithEmail")}" + " :" + string.Join(",", invalidusers));
                }
                var groupMemberDto = new List<GroupMember>();
                var existingGroupUsers = new List<GroupMember>();
                var courseMembers = await _unitOfWork.GetRepository<CourseEnrollment>().GetAllAsync(predicate: p => validUserIds.Contains(p.UserId) && p.CourseId == course.Id).ConfigureAwait(false);
                if (courseMembers.Count != default)
                {
                    var courseEnrollmentDto = new List<CourseEnrollment>();
                    var enrolledUserIds = courseMembers.Select(x => x.UserId).ToList();
                    var deletedUserIds = new List<Guid>();
                    foreach (var courseMember in courseMembers)
                    {
                        courseMember.UpdatedOn = DateTime.UtcNow;
                        courseMember.UpdatedBy = currentUserId;
                        courseMember.EnrollmentMemberStatus = EnrollmentMemberStatusEnum.Enrolled;
                        if (courseMember.IsDeleted == true)
                        {
                            deletedUserIds.Add(courseMember.UserId);
                            courseMember.IsDeleted = false;
                        }
                        courseEnrollmentDto.Add(courseMember);
                    }
                    _unitOfWork.GetRepository<CourseEnrollment>().Update(courseEnrollmentDto);
                    if (deletedUserIds.Count != default)
                    {
                        message.AppendLine($"{_localizer.GetString("UpdatedDeletedUser")}" + " " + string.Join(" ", userList.Where(x => deletedUserIds.Contains(x.Id)).Select(x => string.IsNullOrWhiteSpace(x.Email) ? x.MobileNumber : x.Email)));
                    }

                    var groupMembers = await _unitOfWork.GetRepository<GroupMember>().GetAllAsync(predicate: p => p.GroupId == course.GroupId && p.IsActive == false
                                     && courseMembers.Select(x => x.UserId).Contains(p.UserId)).ConfigureAwait(false);
                    if(groupMembers?.Count > 0)
                    {
                        existingGroupUsers.AddRange(groupMembers);
                    }
                }

                var newEnrollmentIds = validUserIds.Except(courseMembers.Select(x => x.UserId)).ToList();
                if (newEnrollmentIds.Count != default)
                {
                    var insertCourseEnrollment = new List<CourseEnrollment>();
                    var newGroupMember = new List<GroupMember>();
                    var existingGroupMember = await _unitOfWork.GetRepository<GroupMember>().GetAllAsync(predicate: p => newEnrollmentIds.Contains(p.UserId) && p.GroupId == course.GroupId).ConfigureAwait(false);
                    var newGroupMemberIds = newEnrollmentIds.Except(existingGroupMember.Select(x => x.UserId)).ToList();
                    if (existingGroupMember != default)
                    {
                        existingGroupUsers.AddRange(existingGroupMember);
                    }

                    if (newGroupMemberIds != default)
                    {
                        foreach (var newGroupMemberId in newGroupMemberIds)
                        {
                            if (course.GroupId.HasValue)
                            {
                                newGroupMember.Add(new GroupMember
                                {
                                    Id = Guid.NewGuid(),
                                    UserId = newGroupMemberId,
                                    GroupId = course.GroupId.Value,
                                    IsActive = true,
                                    CreatedBy = currentUserId,
                                    CreatedOn = DateTime.UtcNow,
                                    UpdatedBy = currentUserId,
                                    UpdatedOn = DateTime.UtcNow

                                });
                            }
                        }
                        await _unitOfWork.GetRepository<GroupMember>().InsertAsync(newGroupMember).ConfigureAwait(false);
                    }
                    foreach (var newUserId in newEnrollmentIds)
                    {
                        insertCourseEnrollment.Add(new CourseEnrollment
                        {
                            Id = Guid.NewGuid(),
                            CourseId = course.Id,
                            EnrollmentDate = DateTime.UtcNow,
                            CreatedBy = currentUserId,
                            CreatedOn = DateTime.UtcNow,
                            UserId = newUserId,
                            UpdatedBy = currentUserId,
                            UpdatedOn = DateTime.UtcNow,
                            EnrollmentMemberStatus = EnrollmentMemberStatusEnum.Enrolled,
                        });
                    }
                    if (insertCourseEnrollment.Count != default)
                    {
                        await _unitOfWork.GetRepository<CourseEnrollment>().InsertAsync(insertCourseEnrollment);
                        if (insertCourseEnrollment.Count == 1)
                        {
                            message.AppendLine($"{_localizer.GetString("EnrollmentFor")}" + " " + string.Join(",", userList.Where(x => insertCourseEnrollment.Select(y => y.UserId).Contains(x.Id)).Select(x => x.FullName)) + " "
                                 + $"{_localizer.GetString("HasBeenSuccessful")}");
                        }
                        if (insertCourseEnrollment.Count > 1)
                        {
                            message.AppendLine($"{_localizer.GetString("EnrollmentFor")}" + " " + insertCourseEnrollment.Count + " " + $"{_localizer.GetString("Users")}" + " "
                                + $"{_localizer.GetString("HasBeenSuccessful")}");
                        }
                    }
                }

                if (existingGroupUsers.Count > 0)
                {
                    var updateGroupUsers = existingGroupUsers.DistinctBy(x => x.UserId).ToList();
                    updateGroupUsers.ForEach(x =>
                    {
                        x.IsActive = true;
                        x.UpdatedOn = DateTime.UtcNow;
                        x.UpdatedBy = currentUserId;
                    });
                    _unitOfWork.GetRepository<GroupMember>().Update(updateGroupUsers);
                }
                _unitOfWork.GetRepository<GroupMember>().Update(groupMemberDto);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                return message.ToString();
            });
        }

        /// <summary>
        /// get filtered user list for course
        /// </summary>
        /// <param name="critera">enrolled user search critera</param>
        /// <returns>Task completed</returns>
        public async Task<SearchResult<UserResponseModel>> CourseUserSearchAsync(EnrollmentBaseSearchCritera critera)
        {
            return await ExecuteWithResultAsync(async () =>
            {
                var course = await ValidateAndGetCourse(critera.CurrentUserId, critera.CourseIdentity).ConfigureAwait(false);
                var predicate = PredicateBuilder.New<User>(true);
                predicate = predicate.And(p => p.Role == UserRole.Trainee || p.Role == UserRole.Trainer);
                if (!string.IsNullOrEmpty(critera.CourseIdentity) && critera.EnrollmentStatus == EnrollmentMemberStatusEnum.Unenrolled)
                {
                    var enrolledUserIds = course.CourseEnrollments.Where(x => x.EnrollmentMemberStatus == EnrollmentMemberStatusEnum.Enrolled).Select(x => x.UserId)
                        .Concat(course.CourseTeachers.Select(x => x.UserId)).ToList();
                    predicate = predicate.And(p => !enrolledUserIds.Contains(p.Id));
                }
                if (!string.IsNullOrEmpty(critera.CourseIdentity) && critera.EnrollmentStatus != EnrollmentMemberStatusEnum.Unenrolled)
                {
                    var enrolledUserIds = course.CourseEnrollments.Where(x => x.EnrollmentMemberStatus == critera.EnrollmentStatus).Select(x => x.UserId)
                        .Concat(course.CourseTeachers.Select(x => x.UserId)).ToList();
                    predicate = predicate.And(p => enrolledUserIds.Contains(p.Id));
                }
                if (!string.IsNullOrEmpty(critera.Search))
                {
                    var search = critera.Search.ToLower().Trim();
                    predicate = predicate.And(x =>
                        ((x.FirstName.Trim() + " " + x.MiddleName.Trim()).Trim() + " " + x.LastName.Trim()).Trim().Contains(search)
                     || x.Email.ToLower().Trim().Contains(search)
                     || x.MobileNumber.ToLower().Trim().Contains(search));
                }
                var user = await _unitOfWork.GetRepository<User>().GetAllAsync(predicate: predicate).ConfigureAwait(false);
                var searchResult = user.ToIPagedList(critera.Page, critera.Size);
                var response = new SearchResult<UserResponseModel>
                {
                    Items = new List<UserResponseModel>(),
                    CurrentPage = searchResult.CurrentPage,
                    PageSize = searchResult.PageSize,
                    TotalCount = searchResult.TotalCount,
                    TotalPage = searchResult.TotalPage
                };
                searchResult.Items.ForEach(p => response.Items.Add(new UserResponseModel
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    MiddleName = p.MiddleName,
                    LastName = p.LastName,
                    Email = p.Email,
                    MobileNumber = p.MobileNumber,
                    ImageUrl = p.ImageUrl,
                    FullName = p.FullName,
                }));
                return response;
            });
        }
    }
}

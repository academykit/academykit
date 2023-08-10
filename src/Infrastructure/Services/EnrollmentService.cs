using Lingtren.Application.Common.Dtos;
using Lingtren.Application.Common.Exceptions;
using Lingtren.Application.Common.Interfaces;
using Lingtren.Domain.Entities;
using Lingtren.Domain.Enums;
using Lingtren.Infrastructure.Common;
using Lingtren.Infrastructure.Localization;
using LinqKit;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Lingtren.Infrastructure.Services
{
    public class EnrollmentService:BaseGenericService<User,BaseSearchCriteria>,IEnrollmentService
    {
        public EnrollmentService(IUnitOfWork unitOfWork,ILogger<EnrollmentService> logger, IStringLocalizer<ExceptionLocalizer> localizer) : base(unitOfWork, logger, localizer)
        {
        }

        /// <summary>
        /// enroll user in training
        /// </summary>
        /// <param name="email">user email or mobile number</param>
        /// <param name="currentUserId"></param>
        /// <param name="courseIdentity"></param>
        /// <returns></returns>
        /// <exception cref="EntityNotFoundException"></exception>
        public async Task<string> EnrollUserAsync(IList<string> emails, Guid currentUserId, string courseIdentity)
        {
            return await ExecuteWithResultAsync(async () =>
            {
                var course = await _unitOfWork.GetRepository<Course>().GetFirstOrDefaultAsync(predicate: p => p.Id.ToString() == courseIdentity || p.Slug == courseIdentity).ConfigureAwait(false);
                if (course == default)
                {
                    throw new EntityNotFoundException(_localizer.GetString("LessonNotFound"));
                }
                foreach (var email in emails)
                {
                    string emailPattern = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                           @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                              @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
                    if(!Regex.IsMatch(email,emailPattern))
                    {
                        throw new ForbiddenException($"{_localizer.GetString("InvalidEmailFormat")}"+ "," + email);
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
                    message.AppendLine($"{_localizer.GetString("InvalidEmailFormat")}" + " " + string.Join(",", invalidusers));
                }
                var groupMemberDto = new List<GroupMember>();
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
                    message.AppendLine($"{_localizer.GetString("AlreadyEnrolledUser")}" + " " + string.Join(",", userList.Where(x => courseEnrollmentDto.Select(y => y.UserId).Contains(x.Id)).Select(x => string.IsNullOrWhiteSpace(x.Email) ? x.MobileNumber : x.Email)));
                    if (deletedUserIds.Count != default)
                    {
                        message.AppendLine($"UpdatedDeletedUser" + " " + string.Join(" ", userList.Where(x => deletedUserIds.Contains(x.Id)).Select(x => string.IsNullOrWhiteSpace(x.Email) ? x.MobileNumber : x.Email)));
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
                        foreach (var member in existingGroupMember)
                        {
                            member.IsActive = true;
                            member.UpdatedOn = DateTime.UtcNow;
                            member.UpdatedBy = currentUserId;
                            groupMemberDto.Add(member);
                        }
                        _unitOfWork.GetRepository<GroupMember>().Update(groupMemberDto);
                        message.AppendLine($"updatedUser" + " " + string.Join(",", userList.Where(x => existingGroupMember.Select(y => y.UserId).Contains(x.Id)).Select(x => string.IsNullOrWhiteSpace(x.Email) ? x.MobileNumber : x.Email)));
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
                        message.AppendLine($"NewGroupMember" + " " + string.Join(",", userList.Where(x => newGroupMember.Select(y => y.UserId).Contains(x.Id)).Select(x => string.IsNullOrWhiteSpace(x.Email) ? x.MobileNumber : x.Email)));
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
                    }
                }
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                return message.ToString();
            });
        }
    }
}

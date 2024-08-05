﻿using AcademyKit.Domain.Enums;

namespace AcademyKit.Application.Common.Dtos
{
    public class CourseBaseSearchCriteria : BaseSearchCriteria
    {
        /// <summary>
        /// Gets or sets the enrollment status such as Enrolled, Host, moderator, NotEnrolled
        /// </summary>
        public IList<CourseEnrollmentStatus> EnrollmentStatus { get; set; }
        public bool? IncludePast { get; set; }
        public CourseStatus? Status { get; set; }
        public Guid UserId { get; set; }
    }

    /// <summary>
    /// Represent the enrollment type of a user on current course
    /// </summary>
    public enum CourseEnrollmentStatus
    {
        Author = 1,
        Enrolled = 2,
        NotEnrolled = 3,
        Teacher = 4,
    }
}

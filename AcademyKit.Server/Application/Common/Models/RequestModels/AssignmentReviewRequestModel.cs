﻿namespace AcademyKit.Application.Common.Models.RequestModels
{
    public class AssignmentReviewRequestModel
    {
        public Guid UserId { get; set; }
        public decimal Marks { get; set; }
        public string Review { get; set; }
        public bool IsPassed { get; set; }
    }
}

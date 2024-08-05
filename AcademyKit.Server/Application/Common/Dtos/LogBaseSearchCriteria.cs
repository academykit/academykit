﻿using AcademyKit.Domain.Enums;

namespace AcademyKit.Application.Common.Dtos
{
    public class LogBaseSearchCriteria : BaseSearchCriteria
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public SeverityType? Severity { get; set; }
    }
}

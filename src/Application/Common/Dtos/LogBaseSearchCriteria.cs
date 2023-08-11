﻿using Lingtren.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lingtren.Application.Common.Dtos
{
    public class LogBaseSearchCriteria : BaseSearchCriteria
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public SeverityType? Severity { get; set; }
    }
}

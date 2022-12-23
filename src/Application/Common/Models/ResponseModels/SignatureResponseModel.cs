using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class SignatureResponseModel
    {
        public Guid Id { get; set; }
        public string CourseIdentity { get; set; }
        public string Designation { get; set; }
        public string PersonName { get; set; }
        public string SignatureURL { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}

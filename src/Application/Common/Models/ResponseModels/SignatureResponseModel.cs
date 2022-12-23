using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lingtren.Application.Common.Models.ResponseModels
{
    public class SignatureResponseModel
    {
        public string CourseIdentity { get; set; }
        public string Designation { get; set; }
        public string PersonName { get; set; }
        public string SignatureURL { get; set; }
    }
}

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lingtren.Application.Common.Models.RequestModels
{
    public  class SignatureRequestModel
    {
        
        public string CourseIdentity { get; set; }
        public IFormFile File { get; set; }
        public string Designation { get; set; }
        public string PersonName { get; set; }
    }
}

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lingtren.Application.Common.Models.RequestModels
{
    public  class SignatureFileRequestModel
    {
        public string FileURL { get; set; }
        public string Designation { get; set; }
        public string PersonName { get; set; }
    }
}

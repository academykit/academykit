// <copyright file="UtilityController.cs" company="Vurilo Nepal Pvt. Ltd.">
// Copyright (c) Vurilo Nepal Pvt. Ltd.. All rights reserved.
// </copyright>

namespace Lingtren.Api.Controllers
{
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Domain.Entities;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class UtilityController : BaseApiController
    {
        private readonly IDynamicImageGenerator dynamicImageGenerator;

        public UtilityController(IDynamicImageGenerator dynamicImageGenerator)
        {
            this.dynamicImageGenerator = dynamicImageGenerator;
        }

        [HttpGet("ogimage")]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateOpenGraphImage(
            string title,
            string author,
            string image,
            string logo,
            string company
        )
        {
            var bytes = await dynamicImageGenerator.GenerateOgImage(
                title,
                author,
                image,
                logo,
                company
            );

            Response.Headers["Content-Type"] = "image/png";
            Response.Headers["Cache-Control"] =
                "public, immutable, no-transform, s-maxage=31536000, max-age=31536000";

            return File(bytes, "image/png");
        }

        [HttpGet("certificate")]
        [AllowAnonymous]
        public IActionResult GenerateCertificate()
        {
            var companyLogo = "https://i.ibb.co/0jZzQYH/Group-1.png";
            var companyName = "Lingtren Pvt . Ltd .";
            var name = "Aryan Phuyal";
            var training =
                "advanceReact js advanceReact js advanceReact js advanceReact js advanceReact js advanceReact js advanceReact js advanceReact js advanceReact js";
            var startDate = "15th June, 2022";
            var endDate = "17th June, 2022";
            var authors = new List<Signature>
            {
                new Signature
                {
                    FileUrl =
                        "https://static.cdn.wisestamp.com/wp-content/uploads/2020/08/Michael-Jordan-personal-autograph.png",
                    FullName = "Alina KC",
                    Designation = "Managing Directorrrrrrrrrr r",
                },
                new Signature
                {
                    FullName = "Aryan Phuyal Aryan",
                    Designation = "Managing Director",
                    FileUrl =
                        "https://static.cdn.wisestamp.com/wp-content/uploads/2020/08/Oprah-Winfrey-Signature-1.png"
                },
            };

            var bytes = dynamicImageGenerator.GetCertificateHtml(
                companyLogo,
                companyName,
                name,
                training,
                startDate,
                endDate,
                authors
            );

            return Content(bytes, "text/html");
        }
    }
}

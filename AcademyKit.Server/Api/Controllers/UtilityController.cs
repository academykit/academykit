namespace AcademyKit.Api.Controllers
{
    using AcademyKit.Application.Common.Interfaces;
    using AcademyKit.Domain.Entities;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class UtilityController : BaseApiController
    {
        private readonly IDynamicImageGenerator dynamicImageGenerator;

        public UtilityController(IDynamicImageGenerator dynamicImageGenerator)
        {
            this.dynamicImageGenerator = dynamicImageGenerator;
        }

        [HttpGet("ogImage")]
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
            var companyName = "AcademyKit";
            var name = "Aryan Thomas";
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
                    FullName = "Thomas KC",
                    Designation = "Managing Director",
                },
                new Signature
                {
                    FullName = "Aryan Thomas",
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

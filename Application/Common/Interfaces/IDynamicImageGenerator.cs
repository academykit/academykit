﻿using AcademyKit.Domain.Entities;

namespace AcademyKit.Application.Common.Interfaces
{
    public interface IDynamicImageGenerator
    {
        Task<byte[]> GenerateOgImage(
            string title,
            string author,
            string image,
            string logo,
            string company
        );
        Task<Stream> GenerateCertificateImage(
            CourseCertificate certificate,
            string fullName,
            IList<Signature> signatures,
            GeneralSetting company
        );

        string GetCertificateHtml(
            string companyLogo,
            string companyName,
            string name,
            string training,
            string startDate,
            string endDate,
            IList<Signature> authors
        );
    }
}

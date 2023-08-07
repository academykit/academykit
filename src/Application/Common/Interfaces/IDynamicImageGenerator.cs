using Lingtren.Domain.Entities;

namespace Lingtren.Application.Common.Interfaces
{
    public interface IDynamicImageGenerator
    {
        Task<byte[]> GenerateOgImage(String title, String author, String image, String logo, String company);
        Task<Stream> GenerateCertificateImage(CourseCertificate? certificate, string fullName, IList<Signature> signatures, GeneralSetting company);

        string GetCertificateHtml(string companyLogo, string companyName, string name, string training,
        string startDate, string endDate, IList<Signature> authors);
    }
}
namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;

    public class ZoomLicenseResponseModel
    {
        public Guid Id { get; set; }
        public string LicenseEmail { get; set; }
        public string HostId { get; set; }
        public int Capacity { get; set; }
        public bool IsActive { get; set; }
        public UserModel User { get; set; }
        public ZoomLicenseResponseModel(ZoomLicense model)
        {
            Id = model.Id;
            LicenseEmail = model.LicenseEmail;
            HostId = model.HostId;
            Capacity = model.Capacity;
            IsActive = model.IsActive;
            User = model.User != null ? new UserModel(model.User) : new UserModel();
        }
        public ZoomLicenseResponseModel()
        {

        }
    }
}

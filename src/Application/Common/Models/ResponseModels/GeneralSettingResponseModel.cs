namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;

    public class GeneralSettingResponseModel
    {
        public Guid Id { get; set; }
        public string LogoUrl { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyContactNumber { get; set; }
        public string EmailSignature { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public UserModel User { get; set; }
        public string CustomConfiguration { get; set; }
        public GeneralSettingResponseModel(GeneralSetting model)
        {
            Id = model.Id;
            LogoUrl = model.LogoUrl;
            CompanyName = model.CompanyName;
            CompanyAddress = model.CompanyAddress;
            CompanyContactNumber = model.CompanyContactNumber;
            EmailSignature = model.EmailSignature;
            UpdatedOn = model.UpdatedOn;
            User = model.User != null ? new UserModel(model.User) : new UserModel();
            CustomConfiguration = model.CustomConfiguration;
        }
    }
}
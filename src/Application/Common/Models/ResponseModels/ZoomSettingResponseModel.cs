namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;

    public class ZoomSettingResponseModel
    {
        public Guid Id { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public string SdkKey { get; set; }
        public string SdkSecret { get; set; }
        public string WebhookSecret { get; set; }
        public string WebHookVerificationKey { get; set; }
        public bool IsRecordingEnabled { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public UserModel User { get; set; }
        public ZoomSettingResponseModel(ZoomSetting model)
        {
            Id = model.Id;
            ApiKey = model.ApiKey;
            ApiSecret = model.ApiSecret;
            SdkKey = model.SdkKey;
            SdkSecret = model.SdkSecret;
            IsRecordingEnabled = model.IsRecordingEnabled;
            UpdatedOn = model.UpdatedOn;
            WebhookSecret = model.WebHookSecret;
            WebHookVerificationKey = model.WebHookVerificationKey;
            User = model.User != null ? new UserModel(model.User) : new UserModel();
        }
    }
}

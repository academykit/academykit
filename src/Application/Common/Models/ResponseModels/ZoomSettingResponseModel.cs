namespace Lingtren.Application.Common.Models.ResponseModels
{
    using Lingtren.Domain.Entities;

    public class ZoomSettingResponseModel
    {
        public Guid Id { get; set; }
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
        public bool IsRecordingEnabled { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public UserResponseModel User { get; set; }
        public ZoomSettingResponseModel(ZoomSetting model)
        {
            Id = model.Id;
            ApiKey = model.ApiKey;
            SecretKey = model.SecretKey;
            IsRecordingEnabled = model.IsRecordingEnabled;
            UpdatedOn = model.UpdatedOn;
            User = model.User != null ? new UserResponseModel(model.User) : new UserResponseModel();
        }
    }
}

namespace Application.Common.Models.ResponseModels
{
    using Newtonsoft.Json;

    public class LicenseKey
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("activation_limit")]
        public int ActivationLimit { get; set; }

        [JsonProperty("activation_usage")]
        public int ActivationUsage { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("expires_at")]
        public DateTime? ExpiresAt { get; set; }

        [JsonProperty("test_mode")]
        public bool TestMode { get; set; }
    }

    public class Meta
    {
        [JsonProperty("store_id")]
        public int StoreId { get; set; }

        [JsonProperty("order_id")]
        public int OrderId { get; set; }

        [JsonProperty("order_item_id")]
        public int OrderItemId { get; set; }

        [JsonProperty("variant_id")]
        public int VariantId { get; set; }

        [JsonProperty("variant_name")]
        public string VariantName { get; set; }

        [JsonProperty("product_id")]
        public int ProductId { get; set; }

        [JsonProperty("product_name")]
        public string ProductName { get; set; }

        [JsonProperty("customer_id")]
        public int CustomerId { get; set; }

        [JsonProperty("customer_name")]
        public string CustomerName { get; set; }

        [JsonProperty("customer_email")]
        public string CustomerEmail { get; set; }
    }

    public class LicenseResponseModel
    {
        [JsonProperty("activated")]
        public bool Activated { get; set; }

        [JsonProperty("valid")]
        public bool Valid { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("license_key")]
        public LicenseKey LicenseKey { get; set; }

        [JsonProperty("instance")]
        public object Instance { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }
}
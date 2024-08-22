namespace Application.Common.Models.ResponseModels
{
    public class LicenseKey
    {
        public int id { get; set; }

        public string status { get; set; }

        public string key { get; set; }

        public int activation_limit { get; set; }

        public int activation_usage { get; set; }

        public DateTime created_at { get; set; }

        public DateTime? expires_at { get; set; }

        public bool test_mode { get; set; }
    }

    public class Meta
    {
        public int store_id { get; set; }

        public int order_id { get; set; }

        public int order_item_id { get; set; }

        public int variant_id { get; set; }

        public string variant_name { get; set; }

        public int ProductId { get; set; }

        public string product_name { get; set; }

        public int customer_id { get; set; }

        public string customer_name { get; set; }

        public string customer_email { get; set; }
    }

    public class LicenseResponseModel
    {
        public bool activated { get; set; }

        public bool valid { get; set; }

        public string error { get; set; }

        public LicenseKey license_key { get; set; }

        public object instance { get; set; }

        public Meta meta { get; set; }
    }
}
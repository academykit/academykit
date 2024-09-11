using Newtonsoft.Json;

namespace AcademyKit.Application.Common.Models.ResponseModels;

/// <summary>
/// Represents the response model for Lemon Squeezy API.
/// </summary>
public class LemonSqueezyResponseModel
{
    /// <summary>
    /// Gets or sets a value indicating whether the license is activated.
    /// </summary>
    public bool Activated { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the license is valid.
    /// </summary>
    public bool Valid { get; set; }

    /// <summary>
    /// Gets or sets the error message, if any.
    /// </summary>
    public string Error { get; set; }

    /// <summary>
    /// Gets or sets the license key information.
    /// </summary>
    [JsonProperty("license_key")]
    public LicenseKey LicenseKey { get; set; }

    /// <summary>
    /// Gets or sets the instance object.
    /// </summary>
    public object Instance { get; set; }

    /// <summary>
    /// Gets or sets the metadata associated with the response.
    /// </summary>
    public Meta Meta { get; set; }
}

/// <summary>
/// Represents the license key information.
/// </summary>
public class LicenseKey
{
    /// <summary>
    /// Gets or sets the unique identifier of the license key.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the status of the license key.
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Gets or sets the actual license key.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of activations allowed for this license key.
    /// </summary>
    [JsonProperty("activation_limit")]
    public int ActivationLimit { get; set; }

    /// <summary>
    /// Gets or sets the current number of activations for this license key.
    /// </summary>
    [JsonProperty("activation_usage")]
    public int ActivationUsage { get; set; }

    /// <summary>
    /// Gets or sets the creation date of the license key.
    /// </summary>
    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the expiration date of the license key.
    /// </summary>
    [JsonProperty("expires_at")]
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the license key is in test mode.
    /// </summary>
    [JsonProperty("test_mode")]
    public bool TestMode { get; set; }
}

/// <summary>
/// Represents the metadata associated with the Lemon Squeezy response.
/// </summary>
public class Meta
{
    /// <summary>
    /// Gets or sets the creation date of the metadata.
    /// </summary>
    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the store identifier.
    /// </summary>
    [JsonProperty("store_id")]
    public int StoreId { get; set; }

    /// <summary>
    /// Gets or sets the order identifier.
    /// </summary>
    [JsonProperty("order_id")]
    public int OrderId { get; set; }

    /// <summary>
    /// Gets or sets the order item identifier.
    /// </summary>
    [JsonProperty("order_item_id")]
    public int OrderItemId { get; set; }

    /// <summary>
    /// Gets or sets the variant identifier.
    /// </summary>
    [JsonProperty("variant_id")]
    public int VariantId { get; set; }

    /// <summary>
    /// Gets or sets the name of the variant.
    /// </summary>
    [JsonProperty("variant_name")]
    public string VariantName { get; set; }

    /// <summary>
    /// Gets or sets the product identifier.
    /// </summary>
    [JsonProperty("product_id")]
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the name of the product.
    /// </summary>
    [JsonProperty("product_name")]
    public string ProductName { get; set; }

    /// <summary>
    /// Gets or sets the customer identifier.
    /// </summary>
    [JsonProperty("customer_id")]
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the name of the customer.
    /// </summary>
    [JsonProperty("customer_name")]
    public string CustomerName { get; set; }

    /// <summary>
    /// Gets or sets the customer's email address.
    /// </summary>
    [JsonProperty("customer_email")]
    public string CustomerEmail { get; set; }
}

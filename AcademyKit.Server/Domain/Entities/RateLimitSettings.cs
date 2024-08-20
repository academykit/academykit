namespace AcademyKit.Domain.Entities
{
    public class RateLimitSettings
    {
        public const string RateLimit = "RateLimit";
        public int PermitLimit { get; set; } = 2;
        public int Window { get; set; } = 10;
        public int SegmentsPerWindow { get; set; } = 1;
    }
}

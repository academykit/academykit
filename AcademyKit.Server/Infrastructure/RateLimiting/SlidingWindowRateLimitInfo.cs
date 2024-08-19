namespace AcademyKit.Infrastructure.RateLimiting
{
    // Sliding window rate limit information class
    public class SlidingWindowRateLimitInfo
    {
        public int Limit { get; set; }
        public TimeSpan Window { get; set; }
        public int Segments { get; set; }
        public int[] RequestCounts { get; set; }
        public int CurrentSegment { get; set; }
        public DateTimeOffset LastAccessed { get; set; }

        public int TotalRequests => CalculateTotalRequests();
        public DateTimeOffset NextReset => LastAccessed.Add(Window / Segments);

        private int CalculateTotalRequests()
        {
            var total = 0;
            foreach (var count in RequestCounts)
            {
                total += count;
            }

            return total;
        }
    }
}

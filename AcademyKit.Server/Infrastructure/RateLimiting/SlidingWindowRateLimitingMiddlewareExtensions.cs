namespace AcademyKit.Infrastructure.RateLimiting
{
    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class SlidingWindowRateLimitingMiddlewareExtensions
    {
        public static IApplicationBuilder UseSlidingWindowRateLimiting(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SlidingWindowRateLimitingMiddleware>();
        }
    }
}
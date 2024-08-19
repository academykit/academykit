using System.Collections.Concurrent;
using System.Globalization;
using AcademyKit.Domain.Entities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AcademyKit.Infrastructure.RateLimiting
{
    public class SlidingWindowRateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SlidingWindowRateLimitingMiddleware> _logger;
        private readonly RateLimitSettings _rateLimitSettings;
        private static readonly ConcurrentDictionary<
            string,
            SlidingWindowRateLimitInfo
        > _rateLimits = new();

        public SlidingWindowRateLimitingMiddleware(
            RequestDelegate next,
            ILogger<SlidingWindowRateLimitingMiddleware> logger,
            IOptions<RateLimitSettings> rateLimitSettings
        )
        {
            _next = next;
            _logger = logger;
            _rateLimitSettings = rateLimitSettings.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var apiKey = context.Request.Headers["X-API-KEY"].ToString();

            if (string.IsNullOrEmpty(apiKey))
            {
                await _next(context); // No API key, proceed with the request (no rate limiting)
                return;
            }

            var now = DateTimeOffset.UtcNow;

            var rateLimitInfo = _rateLimits.GetOrAdd(
                apiKey,
                _ => new SlidingWindowRateLimitInfo
                {
                    Limit = _rateLimitSettings.PermitLimit,
                    Window = TimeSpan.FromSeconds(_rateLimitSettings.Window),
                    Segments = _rateLimitSettings.SegmentsPerWindow,
                    RequestCounts = new int[_rateLimitSettings.SegmentsPerWindow],
                    LastAccessed = now
                }
            );

            lock (rateLimitInfo)
            {
                // Slide the window
                SlideWindow(rateLimitInfo, now);

                // Calculate total requests in the current window
                var totalRequests = rateLimitInfo.TotalRequests;

                // Set rate limiting headers
                context.Response.Headers["X-RateLimit-Limit"] = rateLimitInfo.Limit.ToString(
                    CultureInfo.InvariantCulture
                );
                context.Response.Headers["X-RateLimit-Reset"] = rateLimitInfo
                    .NextReset.ToUnixTimeSeconds()
                    .ToString(CultureInfo.InvariantCulture);

                if (totalRequests < rateLimitInfo.Limit)
                {
                    // Allow request and increment the current segment
                    rateLimitInfo.RequestCounts[rateLimitInfo.CurrentSegment]++;
                    context.Response.Headers["X-RateLimit-Remaining"] = (
                        rateLimitInfo.Limit - rateLimitInfo.TotalRequests
                    ).ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    // Rate limit exceeded
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.Response.Headers["X-RateLimit-Remaining"] = (
                        rateLimitInfo.Limit - rateLimitInfo.TotalRequests
                    ).ToString(CultureInfo.InvariantCulture);
                    var timeSpanPerSegment = rateLimitInfo.Window / rateLimitInfo.Segments;

                    var retryAfter =
                        rateLimitInfo.LastAccessed.Add(
                            CalculateSegmentsToWait(rateLimitInfo) * timeSpanPerSegment
                        ) - now;
                    context.Response.Headers.RetryAfter = ((int)retryAfter.TotalSeconds).ToString(
                        NumberFormatInfo.InvariantInfo
                    );
                    _logger.LogWarning("Rate limit exceeded for API key {ApiKey}.", apiKey);
                    return;
                }
            }

            await _next(context);
        }

        private static int CalculateSegmentsToWait(SlidingWindowRateLimitInfo rateLimitInfo)
        {
            var segmentsToWait = 1;
            while (
                rateLimitInfo.RequestCounts[
                    (rateLimitInfo.CurrentSegment + segmentsToWait) % rateLimitInfo.Segments
                ] == 0
            )
            {
                segmentsToWait++;
            }

            return segmentsToWait;
        }

        private static void SlideWindow(
            SlidingWindowRateLimitInfo rateLimitInfo,
            DateTimeOffset now
        )
        {
            var timeSinceLastAccess = now - rateLimitInfo.LastAccessed;

            if (timeSinceLastAccess >= rateLimitInfo.Window / rateLimitInfo.Segments)
            {
                var segmentsToSlide = (int)(
                    timeSinceLastAccess / (rateLimitInfo.Window / rateLimitInfo.Segments)
                );
                rateLimitInfo.LastAccessed = rateLimitInfo.LastAccessed.Add(
                    segmentsToSlide * (rateLimitInfo.Window / rateLimitInfo.Segments)
                );

                for (var i = 0; i < segmentsToSlide && i < rateLimitInfo.Segments; i++)
                {
                    rateLimitInfo.CurrentSegment =
                        (rateLimitInfo.CurrentSegment + 1) % rateLimitInfo.Segments;
                    rateLimitInfo.RequestCounts[rateLimitInfo.CurrentSegment] = 0;
                }
            }
        }
    }
}

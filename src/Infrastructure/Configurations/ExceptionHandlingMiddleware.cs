namespace Lingtren.Infrastructure.Configurations
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using FluentValidation;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;

        /// <summary>
        /// Initializes the new instance of <see cref="ExceptionHandlingMiddleware"/>.
        /// </summary>
        /// <param name="next">The request to handle</param>
        public ExceptionHandlingMiddleware(RequestDelegate next) => this.next = next;

        /// <summary>
        /// Invokes the middle-ware handler.
        /// </summary>
        /// <param name="context">The current http context.</param>
        /// <param name="logger">The logger</param>
        public async Task Invoke(HttpContext context, ILogger<ExceptionHandlingMiddleware> logger)
        {
            try
            {
                await next(context).ConfigureAwait(false);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                await HandleExceptionAsync(context, ex, logger).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <param name="context">The current http context.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="logger">The logger instance.</param>
        private static async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception,
            ILogger<ExceptionHandlingMiddleware> logger
        )
        {
            var isNotDevelopment =
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                != Environments.Development;

            HttpStatusCode statusCode;
            object message;
            switch (exception)
            {
                case SecurityException:
                    statusCode = HttpStatusCode.Forbidden;

                    message = new ApiError { Message = "You shall not pass!" };

                    break;
                case ArgumentException argumentInValid:
                    statusCode = HttpStatusCode.BadRequest;

                    message = new ApiError { Message = argumentInValid.Message };
                    break;
                case ValidationException validation:
                    statusCode = HttpStatusCode.BadRequest;

                    message = new ApiError
                    {
                        Message = validation.Errors.FirstOrDefault()?.ErrorMessage
                    };

                    break;
                case ForbiddenException forbidden:
                    statusCode = HttpStatusCode.Forbidden;

                    message = new ApiError
                    {
                        Message =
                            forbidden.Message ?? "You are not allowed to access this resource."
                    };
                    break;
                case EntityNotFoundException entityNotFound:
                    statusCode = HttpStatusCode.NotFound;

                    message = new ApiError
                    {
                        Message =
                            entityNotFound.Message
                            ?? "These aren't the droids you're looking for..."
                    };

                    break;
                case ServiceException serviceError:
                    statusCode = HttpStatusCode.InternalServerError;
                    message = new ApiError { Message = serviceError.Message };
                    break;
                default:
                    statusCode = HttpStatusCode.InternalServerError;

                    var defaultEx = new ApiError
                    {
                        Message = exception.InnerException?.Message ?? exception.Message
                    };

                    if (!isNotDevelopment)
                    {
                        defaultEx.Message =
                            $"Exception: {exception.Message} - Inner: {exception.InnerException?.Message} - Stacktrace: {exception.StackTrace}";
                    }

                    message = defaultEx;
                    break;
            }

            logger.LogError("EXCEPTION HANDLING {statusCode} | {exception}", statusCode, exception);
            DefaultContractResolver contractResolver =
                new() { NamingStrategy = new CamelCaseNamingStrategy() };
            var result = JsonConvert.SerializeObject(
                message,
                new JsonSerializerSettings
                {
                    ContractResolver = contractResolver,
                    Formatting = Formatting.Indented
                }
            );

            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.StatusCode = (int)statusCode;

            await context.Response.WriteAsync(result, Encoding.UTF8).ConfigureAwait(false);
        }
    }
}

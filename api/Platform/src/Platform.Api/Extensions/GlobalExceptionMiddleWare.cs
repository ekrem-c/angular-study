using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Platform.Common;

namespace Platform.Api.Extensions
{
    public class GlobalExceptionMiddleWare
    {
        private readonly ILogger<GlobalExceptionMiddleWare> logger;
        private readonly RequestDelegate next;

        public GlobalExceptionMiddleWare(RequestDelegate next, ILogger<GlobalExceptionMiddleWare> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Unhandled API Exception");
                await HandleExceptionAsync(context, exception);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = (int) HttpStatusCode.InternalServerError; // 500

            // https://en.wikipedia.org/wiki/List_of_HTTP_status_codes
            // 300 - Ambiguous => can use it to redirect to another version (URL resolution?)
            // 301 - Moved => Get
            // 302 - Permanent => Get
            // 303 - See Other => Get
            // 304 - Not Modified
            if (exception is NotModifiedException)
            {
                code = (int) HttpStatusCode.NotModified;
            }
            // 307 - Moved => Keeps Verb
            // 308 - Permanent => Keeps Verb
            // 400 - Bad Request
            else if (exception is BadRequestException)
            {
                code = (int) HttpStatusCode.BadRequest;
            }
            // 401 - Unauthorized => Signin
            else if (exception is UnauthorizedAccessException)
            {
                code = (int) HttpStatusCode.Unauthorized;
            }
            // 402 - Payment Required => if we disable access due to payment?
            // 403 - Forbidden => authorized, but invalid access
            else if (exception is ForbiddenException)
            {
                code = (int) HttpStatusCode.Forbidden;
            }
            // 404 - Not Found => The object was null when returned or threw an error
            else if (exception is NotFoundException)
            {
                code = (int) HttpStatusCode.NotFound;
            }
            // 406 - Not Accepted => Header isn't accepted: Language or ContentType, etc
            // 408 - Request Timeout => DB Timeouts, other?
            // 409 - Conflict => When resource already exists; Concurrency checks; etc
            else if (exception is ConflictException
                || exception is DomainException)
            {
                code = (int) HttpStatusCode.Conflict;
            }
            // 410 - Gone => Requesting an item that is marked as deleted.
            else if (exception is RemovedException)
            {
                code = (int) HttpStatusCode.Gone;
            }
            // 413 - Request Entity Too Large => Size restrictions on upload?
            // 415 - Unsupported Media Type => AcceptedType, ContentType.  May be able to use for file types.
            // 416 - Requested Range Is Not Valid => Paging ranges, indexes (sequence), etc
            // 417 - Expectation Failed
            // 418 - I'm a Teapot => Fun
            // 422 - Unprocessable Entity
            // 423 - Locked => Resource is Locked?
            else if (exception is ReadOnlyException)
            {
                code = 423;
            }
            // 426 - Upgrade Required => Api Version deprecated?
            // 429 - Too Many Requests => Api Throttling
            // 451 - Unavailable for Legal Reasons => Locality Execution Blocking, DMCA, HIPAA, PII Restrictions, etc
            // 500 - Internal Server Error => Configuration, Hardware, Network, Etc.
            else if (exception is InternalServerException)
            {
                code = (int) HttpStatusCode.InternalServerError;
            }
            // 501 - Not Implemented => The Developer isn't done yet!
            else if (exception is NotImplementedException)
            {
                code = (int) HttpStatusCode.NotImplemented;
            }
            // 503 - Service Unavailable => Overloaded, Service Failure, Maintenance Windows.
            else if (exception is ServiceUnavailableException)
            {
                code = (int) HttpStatusCode.ServiceUnavailable;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = code;

            string result;
            if (exception is ExceptionBase exceptionBase
                && exceptionBase.ErrorMessage != null)
            {
                //When the error code is 0 use the HTTP status code instead.
                if (exceptionBase.ErrorMessage.Error.Code == 0)
                {
                    exceptionBase.ErrorMessage.Error.Code = code;
                }

                result = JsonConvert.SerializeObject(exceptionBase.ErrorMessage);
                return context.Response.WriteAsync(result);
            }

            result = JsonConvert.SerializeObject(new {error = exception.Message});
            return context.Response.WriteAsync(result);
        }
    }
}
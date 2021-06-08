using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Platform.Api.Filters
{
    public class ResponseTimeFilter
    {
        private readonly RequestDelegate next;

        public ResponseTimeFilter(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            context.Response.OnStarting(
                () =>
                {
                    stopWatch.Stop();
                    context.Response.Headers.Add("X-ResponseTime", $"{stopWatch.ElapsedMilliseconds.ToString()}ms");
                    return Task.CompletedTask;
                });

            await next(context);
        }
    }
}
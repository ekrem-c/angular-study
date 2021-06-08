using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Platform.Common;

namespace Platform.Api
{
    [Route("healthcheck")]
    public class HealthCheckController : Controller
    {
        private readonly IServiceProvider serviceProvider;

        public HealthCheckController(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        [HttpGet]
        public IActionResult PingAsync()
        {
            return Ok("I’M ALIVE");
        }

        [HttpGet("resources")]
        public async Task<IActionResult> ResourcesAsync()
        {
            var checks = ActivatorUtilities
                .GetServiceOrCreateInstance<IEnumerable<IHealthCheckResource>>(serviceProvider).ToList();

            var results = await Task.WhenAll(checks.Select(x => x.CheckHealthAsync()));

            if (results.All(x => x.Status == HealthCheckStatus.Ok))
            {
                return Ok(results);
            }
            
            return BadRequest(results);
        }
    }
}
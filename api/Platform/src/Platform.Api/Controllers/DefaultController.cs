using Microsoft.AspNetCore.Mvc;

namespace Platform.Api
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("")]
    public class DefaultController : Controller
    {
        [HttpGet]
        public IActionResult PingAsync()
        {
            return RedirectPermanent("~/swagger");
        }
    }
}
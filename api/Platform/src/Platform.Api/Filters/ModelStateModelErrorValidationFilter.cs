using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Platform.Common;

namespace Platform.Api.Extensions
{
    public class ModelStateModelErrorValidationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var list = (from modelState in context.ModelState.Values
                    from error in modelState.Errors
                    select error.ErrorMessage).ToList();
                context.Result = new BadRequestObjectResult(
                    new ModelErrorMessage(new ModelError((int) HttpStatusCode.BadRequest, string.Join(",", list))));
            }

            base.OnActionExecuting(context);
        }
    }
}
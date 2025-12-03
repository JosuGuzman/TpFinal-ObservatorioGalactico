namespace Observatorio.API.Filters;

public class ValidateModelFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(e => e.Value.Errors.Count > 0)
                .Select(e => new
                {
                    Field = e.Key,
                    Errors = e.Value.Errors.Select(err => err.ErrorMessage).ToArray()
                })
                .ToList();

            context.Result = new BadRequestObjectResult(new
            {
                error = "Validation failed",
                message = "One or more validation errors occurred",
                errors
            });
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // No action needed after execution
    }
}
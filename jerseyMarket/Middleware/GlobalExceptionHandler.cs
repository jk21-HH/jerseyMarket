using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace jerseyMarket.Middleware
{
    public class GlobalExceptionHandler(IHostEnvironment env, ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        // add the cancellationToken parameter to the method signature - if backend is abrupted it saves resources
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError(exception, "Unhandled exception occurred.");

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred.",
                Detail = env.IsDevelopment() ? exception.ToString() : null,
            };

            httpContext.Response.StatusCode = problemDetails.Status.Value;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}

namespace Aps.Configuration.Apis.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.UtcNow;

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
            var requestInfo = new
            {
                context.Request.Method,
                context.Request.Path,
                QueryString = context.Request.QueryString.ToString(),
                context.Response.StatusCode
            };

            _logger.LogError(ex, "Unhandled exception occurred. Request Information: {@RequestInfo}. Duration: {Duration} ms.", requestInfo, duration);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        (HttpStatusCode statusCode, string message, List<ValidationErrorDetail> details) = exception switch
        {
            ArgumentException => (
                HttpStatusCode.BadRequest,
                ErrorMessages.InvalidArgument,
                null
            ),
            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized,
                ErrorMessages.AccessDenied,
                null
            ),
            KeyNotFoundException => (
                HttpStatusCode.NotFound,
                ErrorMessages.ResourceNotFound,
                null
            ),
            ValidationException validationException => (
                HttpStatusCode.BadRequest,
                ErrorMessages.ValidationFailed,
                validationException.Errors
                    .Select(e => new ValidationErrorDetail
                    {
                        Property = e.PropertyName,
                        ErrorMessage = e.ErrorMessage
                    })
                    .ToList()
            ),
            TimeoutException => (
                HttpStatusCode.RequestTimeout,
                ErrorMessages.RequestTimeout,
                null
            ),
            HttpRequestException => (
                HttpStatusCode.BadGateway,
                ErrorMessages.NetworkError,
                null
            ),
            MongoWriteException => (
                HttpStatusCode.Conflict,
                ErrorMessages.MongoWriteError,
                null
            ),
            MongoConnectionException => (
                HttpStatusCode.ServiceUnavailable,
                ErrorMessages.MongoConnectionError,
                null
            ),
            MongoException => (
                HttpStatusCode.InternalServerError,
                ErrorMessages.MongoDbError,
                null
            ),
            DataAccessException => (
                HttpStatusCode.InternalServerError,
                exception.Message,
                null
            ),
            _ => (
                HttpStatusCode.InternalServerError,
                ErrorMessages.UnexpectedError,
                null
            )
        };

        var response = new ApiResponse<object>
        {
            Success = false,
            Message = message,
            Data = details,
            TotalDocuments = 0,
            TotalPages = 0,
            PageNumber = 0,
            PageSize = 0
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
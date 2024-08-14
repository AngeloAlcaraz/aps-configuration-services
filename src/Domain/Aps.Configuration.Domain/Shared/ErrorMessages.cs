namespace Aps.Configuration.Core.Shared;

public static class ErrorMessages
{
    public const string InvalidArgument = "Invalid argument provided.";
    public const string AccessDenied = "Access is denied.";
    public const string ResourceNotFound = "Resource not found.";
    public const string ValidationFailed = "Validation failed. Please check the input values.";
    public const string UnexpectedError = "An unexpected error occurred.";
    public const string MongoDbError = "An error occurred while interacting with the MongoDB database.";
    public const string MongoConnectionError = "Failed to connect to the MongoDB database.";
    public const string MongoWriteError = "An error occurred while writing to the MongoDB database.";
    public const string RequestTimeout = "The request timed out.";
    public const string NetworkError = "A network error occurred.";
    public const string RequestNotFound = "The requested operation could not be found.";
    public const string RequestFailed = "The request failed.";
}
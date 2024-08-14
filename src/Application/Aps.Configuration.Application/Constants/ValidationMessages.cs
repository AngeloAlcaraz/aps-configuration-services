namespace Aps.Configuration.Application.Constants;

public static class ValidationMessages
{
    public const string RequiredField = "This field is required.";
    public const string MinimumLength = "The field must be at least {MinLength} characters long.";
    public const string NumericField = "This field should only contain numeric characters.";
    public const string PositiveNumber = "This field must be a positive number.";
    public const string NoResultsMessage = "No results found for the given criteria.";
    public const string PropertyNotFound = "The property name does not exist in the request.";
    public const string SuccessMessage = "Operation completed successfully.";
}
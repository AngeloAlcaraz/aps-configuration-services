namespace Aps.Configuration.Application.Extensions;

public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, string> MustHaveMinimumLength<T>(this IRuleBuilder<T, string> ruleBuilder, int minLength)
    {
        return ruleBuilder
            .NotEmpty().WithMessage(ValidationMessages.RequiredField)
            .MinimumLength(minLength).WithMessage(ValidationMessages.MinimumLength.Replace("{MinLength}", minLength.ToString()));
    }

    public static IRuleBuilderOptions<T, string> MustBeNumeric<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage(ValidationMessages.RequiredField)
            .Must(value => int.TryParse(value, out _)).WithMessage(ValidationMessages.NumericField);
    }

    public static IRuleBuilderOptions<T, string> MustBePositive<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage(ValidationMessages.RequiredField)
            .Must(value =>
            {
                if (!int.TryParse(value, out int number))
                    return false;
                return number > 0;
            }).WithMessage(ValidationMessages.PositiveNumber);
    }
}
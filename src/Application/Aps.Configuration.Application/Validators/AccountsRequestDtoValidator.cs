namespace Aps.Configuration.Application.Validators;

public class AccountsRequestDtoValidator : AbstractValidator<AccountsRequestDto>
{
    private static readonly HashSet<string> ValidSortByProperties = new()
    {
        nameof(AccountsResponseDto.Npi),
        nameof(AccountsResponseDto.Ncpdp),
        nameof(AccountsResponseDto.Bin),
        nameof(AccountsResponseDto.BillCode),
        nameof(AccountsResponseDto.Group),
        nameof(AccountsResponseDto.Region)
    };

    public AccountsRequestDtoValidator()
    {
        RuleFor(account => account.Bin)
            .Cascade(CascadeMode.Stop)
            .Must(BeNumeric)
            .WithMessage("Bin must be a positive numeric value.")
            .Must(BePositive)
            .When(account => !string.IsNullOrEmpty(account.Bin));

        RuleFor(account => account.Nabp)
            .Cascade(CascadeMode.Stop)
            .Must(BeNumeric)
            .WithMessage("Nabp must be a positive numeric value.")
            .Must(BePositive)
            .When(account => !string.IsNullOrEmpty(account.Nabp));

        RuleFor(account => account.Npi)
            .Cascade(CascadeMode.Stop)
            .Must(BeNumeric)
            .WithMessage("Npi must be a positive numeric value.")
            .Must(BePositive)
            .When(account => !string.IsNullOrEmpty(account.Npi));

        RuleFor(account => account.Group)
            .Cascade(CascadeMode.Stop)
            .Must(BeNumeric)
            .WithMessage("Group must be a positive numeric value.")
            .Must(BePositive)
            .When(account => !string.IsNullOrEmpty(account.Group));

        RuleFor(account => account.BillCode)
            .Cascade(CascadeMode.Stop)
            .Must(BeNumeric)
            .WithMessage("BillCode must be a positive numeric value.")
            .Must(BePositive)
            .When(account => !string.IsNullOrEmpty(account.BillCode));

        RuleFor(account => account.Region)
            .Cascade(CascadeMode.Stop)
            .Must(BeNumeric)
            .WithMessage("Region must be a positive numeric value.")
            .Must(BePositive)
            .When(account => !string.IsNullOrEmpty(account.Region));

        RuleFor(account => account.SortBy)
            .Must(sortBy => string.IsNullOrEmpty(sortBy) || ValidSortByProperties.Contains(sortBy))
            .WithMessage(ValidationMessages.PropertyNotFound);
    }

    private bool BeNumeric(string value)
    {
        return long.TryParse(value, out _);
    }

    private bool BePositive(string value)
    {
        if (long.TryParse(value, out var number))
        {
            return number > 0;
        }
        return false;
    }
}
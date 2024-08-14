namespace Aps.Configuration.Application.Filters.AccountsFilters;

public interface IAccountsFilterHandler
{
    void SetNext(IAccountsFilterHandler handler);

    FilterDefinition<AccountDocument> Handle(AccountsRequestDto request);
}
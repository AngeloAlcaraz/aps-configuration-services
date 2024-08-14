namespace Aps.Configuration.Application.Filters.AccountsFilters;

public abstract class AccountsFilterHandler : IAccountsFilterHandler
{
    private IAccountsFilterHandler _nextHandler;

    public void SetNext(IAccountsFilterHandler handler)
    {
        _nextHandler = handler;
    }

    public virtual FilterDefinition<AccountDocument> Handle(AccountsRequestDto request)
    {
        if (_nextHandler is not null)
        {
            return _nextHandler.Handle(request);
        }

        return Builders<AccountDocument>.Filter.Empty;
    }
}
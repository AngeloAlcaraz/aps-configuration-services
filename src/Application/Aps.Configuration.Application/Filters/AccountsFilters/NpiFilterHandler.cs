namespace Aps.Configuration.Application.Filters.AccountsFilters;

public class NpiFilterHandler : AccountsFilterHandler
{
    public override FilterDefinition<AccountDocument> Handle(AccountsRequestDto request)
    {
        var filter = Builders<AccountDocument>.Filter.Empty;

        if (!string.IsNullOrEmpty(request.Npi))
        {
            filter = Builders<AccountDocument>.Filter.Eq("Account.Npi", request.Npi);
        }

        return filter & base.Handle(request);
    }
}
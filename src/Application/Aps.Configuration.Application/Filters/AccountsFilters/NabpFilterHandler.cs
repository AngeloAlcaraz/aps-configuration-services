namespace Aps.Configuration.Application.Filters.AccountsFilters;

public class NabpFilterHandler : AccountsFilterHandler
{
    public override FilterDefinition<AccountDocument> Handle(AccountsRequestDto request)
    {
        var filter = Builders<AccountDocument>.Filter.Empty;

        if (!string.IsNullOrEmpty(request.Nabp))
        {
            filter = Builders<AccountDocument>.Filter.Eq("Account.Ncpdp", request.Nabp);
        }

        return filter & base.Handle(request);
    }
}
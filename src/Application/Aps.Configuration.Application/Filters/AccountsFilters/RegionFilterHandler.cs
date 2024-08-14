namespace Aps.Configuration.Application.Filters.AccountsFilters;

public class RegionFilterHandler : AccountsFilterHandler
{
    public override FilterDefinition<AccountDocument> Handle(AccountsRequestDto request)
    {
        var filter = Builders<AccountDocument>.Filter.Empty;

        if (!string.IsNullOrEmpty(request.Region))
        {
            filter = Builders<AccountDocument>.Filter.Eq("Account.Region", request.Region);
        }

        return filter & base.Handle(request);
    }
}
namespace Aps.Configuration.Application.Filters.AccountsFilters;

public class GroupFilterHandler : AccountsFilterHandler
{
    public override FilterDefinition<AccountDocument> Handle(AccountsRequestDto request)
    {
        if (!string.IsNullOrEmpty(request.Group))
        {
            return Builders<AccountDocument>.Filter.And(
                Builders<AccountDocument>.Filter.Eq("Account.Group", request.Group)
            );
        }
        return base.Handle(request);
    }
}
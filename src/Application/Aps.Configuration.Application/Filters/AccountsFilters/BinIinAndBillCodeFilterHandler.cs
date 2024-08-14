namespace Aps.Configuration.Application.Filters.AccountsFilters;

public class BinIinAndBillCodeFilterHandler : AccountsFilterHandler
{
    public override FilterDefinition<AccountDocument> Handle(AccountsRequestDto request)
    {
        var filter = Builders<AccountDocument>.Filter.Empty;

        if (!string.IsNullOrEmpty(request.Bin))
        {
            filter &= Builders<AccountDocument>.Filter.Eq("Account.Bin", request.Bin);
        }

        if (!string.IsNullOrEmpty(request.BillCode))
        {
            filter &= Builders<AccountDocument>.Filter.Eq("Account.BillCode", request.BillCode);
        }

        if (!string.IsNullOrEmpty(request.Group))
        {
            filter &= Builders<AccountDocument>.Filter.Eq("Account.Group", request.Group);
        }

        return filter & base.Handle(request);
    }
}
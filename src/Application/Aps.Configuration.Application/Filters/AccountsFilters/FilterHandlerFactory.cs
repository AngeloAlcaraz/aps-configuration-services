namespace Aps.Configuration.Application.Filters.AccountsFilters;

public class FilterHandlerFactory : IFilterHandlerFactory
{
    public IAccountsFilterHandler Create()
    {
        var npiFilterHandler = new NpiFilterHandler();
        var nabpFilterHandler = new NabpFilterHandler();
        var binIinAndBillCodeFilterHandler = new BinIinAndBillCodeFilterHandler();
        var groupAndRegionCodeFilterHandler = new GroupFilterHandler();

        npiFilterHandler.SetNext(nabpFilterHandler);
        nabpFilterHandler.SetNext(binIinAndBillCodeFilterHandler);
        binIinAndBillCodeFilterHandler.SetNext(groupAndRegionCodeFilterHandler);

        return npiFilterHandler;
    }
}
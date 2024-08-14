namespace Aps.Configuration.Application.Filters.AccountsFilters;

public interface IFilterHandlerFactory
{
    IAccountsFilterHandler Create();
}
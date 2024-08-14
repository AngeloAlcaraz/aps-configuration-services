namespace Aps.Configuration.Application.Services.AccountsService.Queries;

public class GetSearchAccountsQuery : IRequest<ApiResponse<IEnumerable<AccountsResponseDto>>>
{
    public AccountsRequestDto AccountRequestDto { get; }

    public GetSearchAccountsQuery(AccountsRequestDto accountRequestDto)
    {
        AccountRequestDto = accountRequestDto ?? throw new ArgumentNullException(nameof(accountRequestDto));
    }
}
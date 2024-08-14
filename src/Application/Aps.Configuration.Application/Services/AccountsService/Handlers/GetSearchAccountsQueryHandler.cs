namespace Aps.Configuration.Application.Services.AccountsService.Handlers;

public class GetSearchAccountsQueryHandler : IRequestHandler<GetSearchAccountsQuery, ApiResponse<IEnumerable<AccountsResponseDto>>>
{
    #region Private Variables

    private readonly IRepository<AccountDocument> _accountService;
    private readonly IAccountsFilterHandler _filterHandlerChain;
    private readonly IValidator<AccountsRequestDto> _validator;
    private readonly ILogger<GetSearchAccountsQueryHandler> _logger;

    #endregion Private Variables

    #region Constructor

    public GetSearchAccountsQueryHandler(
        IRepository<AccountDocument> accountService,
        IFilterHandlerFactory filterHandlerFactory,
        IValidator<AccountsRequestDto> validator,
        ILogger<GetSearchAccountsQueryHandler> logger)
    {
        _accountService = accountService;
        _filterHandlerChain = filterHandlerFactory.Create();
        _validator = validator;
        _logger = logger;
    }

    #endregion Constructor

    #region Public Methods

    /// <summary>
    /// Handles the request to retrieve a list of accounts with pagination based on the provided search criteria.
    /// </summary>
    /// <param name="request">The search criteria including filters, pagination details (page number and page size), and sorting options.</param>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>An <see cref="ApiResponse{IEnumerable{AccountsResponseDto}}"/> containing the list of accounts, total count, total pages, and pagination details.</returns>
    public async Task<ApiResponse<IEnumerable<AccountsResponseDto>>> Handle(GetSearchAccountsQuery request, CancellationToken cancellationToken)
    {
#pragma warning disable S2139 // Exceptions should be either logged or rethrown but not both
        try
        {
            await ValidateRequestAsync(request.AccountRequestDto, cancellationToken);

            var filter = _filterHandlerChain.Handle(request.AccountRequestDto);

            int pageNumber = request.AccountRequestDto.PageNumber;
            int pageSize = request.AccountRequestDto.PageSize;

            // Default values for page number and page size
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var (accountDocuments, totalCount) = await _accountService.FindAsync(filter, pageNumber, pageSize);

            var accounts = MapToAccountResponseDtos(accountDocuments);
            accounts = accounts.SortByProperty(request.AccountRequestDto.SortBy, request.AccountRequestDto.SortDescending).ToList();

            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return CreateApiResponse(accounts, totalCount, totalPages, pageNumber, pageSize);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for request: {Request}", JsonSerializer.Serialize(request.AccountRequestDto));
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while handling request {Request}", JsonSerializer.Serialize(request.AccountRequestDto));
            throw;
        }
#pragma warning restore S2139 // Exceptions should be either logged or rethrown but not both
    }

    #endregion Public Methods

    #region Private Methods

    /// <summary>
    /// Maps a collection of account documents to a collection of account response DTOs.
    /// </summary>
    /// <param name="accountDocuments">The collection of account documents to map.</param>
    /// <returns>A collection of <see cref="AccountsResponseDto"/> representing the mapped account documents.</returns>
    private static IEnumerable<AccountsResponseDto> MapToAccountResponseDtos(IEnumerable<AccountDocument> accountDocuments)
    {
        return accountDocuments.SelectMany(accountDocument =>
            accountDocument.Account.Select(account => new AccountsResponseDto
            {
                Ncpdp = account.Ncpdp,
                Npi = account.Npi,
                Bin = account.Bin,
                BillCode = account.BillCode,
                Group = account.Group,
                Region = account.Region
            }));
    }

    /// <summary>
    /// Validates the request using the provided validator.
    /// </summary>
    /// <param name="request">The request to validate.</param>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <exception cref="ValidationException">Thrown when the request is invalid.</exception>
    private async Task ValidateRequestAsync(AccountsRequestDto request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
    }

    /// <summary>
    /// Creates an API response with the provided account data and pagination details.
    /// </summary>
    /// <param name="accounts">The list of account response DTOs.</param>
    /// <param name="totalCount">The total number of documents available.</param>
    /// <param name="totalPages">The total number of pages based on the page size.</param>
    /// <param name="pageNumber">The current page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>An <see cref="ApiResponse{IEnumerable{AccountsResponseDto}}"/> containing the account data and pagination details.</returns>
    private static ApiResponse<IEnumerable<AccountsResponseDto>> CreateApiResponse(IEnumerable<AccountsResponseDto> accounts, int totalCount, int totalPages, int pageNumber, int pageSize)
    {
        return new ApiResponse<IEnumerable<AccountsResponseDto>>
        {
            Success = true,
            Data = accounts.Any() ? accounts : null,
            Message = accounts.Any() ? ValidationMessages.SuccessMessage : ValidationMessages.NoResultsMessage,
            TotalDocuments = totalCount,
            TotalPages = totalPages,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    #endregion Private Methods
}
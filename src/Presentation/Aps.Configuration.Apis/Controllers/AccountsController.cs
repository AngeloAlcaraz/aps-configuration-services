namespace Aps.Configuration.Apis.Controllers;

[Route("aps/configuration/api/v1/accounts")]
[ApiController]
public class AccountsController : ControllerBase
{
    #region Private Variables

    private readonly IMediator _mediator;
    private readonly ILogger<AccountsController> _logger;

    #endregion Private Variables

    #region Constructor

    public AccountsController(IMediator mediator, ILogger<AccountsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    #endregion Constructor

    #region Public Methods

    /// <summary>
    /// Retrieves a list of accounts based on the search criteria provided in the request.
    /// </summary>
    /// <param name="request">The search criteria for retrieving accounts. Includes pagination details (page number and page size),
    /// sorting options, and other filters.</param>
    /// <returns>An <see cref="IActionResult"/> containing the API response with the list of accounts, total count, and pagination details.</returns>
    [HttpGet("get/search")]
    public async Task<IActionResult> GetSearchAccountsAsync([FromQuery] AccountsRequestDto request)
    {
        _logger.LogInformation("Entering AccountsController.GetSearchAccountsAsync()");

        var result = await _mediator.Send(new GetSearchAccountsQuery(request));

        _logger.LogInformation("Exiting AccountsController.GetSearchAccountsAsync()");
        return Ok(result);
    }

    #endregion Public Methods
}
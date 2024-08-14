namespace Aps.Configuration.Infrastructure.Policies;

public interface IPolicyProvider
{
    IAsyncPolicy GetRetryPolicy();
}
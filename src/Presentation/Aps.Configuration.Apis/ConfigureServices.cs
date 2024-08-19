namespace Aps.Configuration.Apis;

public static class ConfigureServices
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        var vaultUri = configuration["VaultSettings:Uri"];
        var vaultClient = new SecretClient(new Uri(vaultUri), new DefaultAzureCredential());

        var connectionStringSecretName = configuration["MongoDbSettings:ConnectionStringSecretName"];
        var databaseSecretName = configuration["MongoDbSettings:DatabaseSecretName"];

        string connectionString = vaultClient.GetSecret(connectionStringSecretName).Value.Value.ToString();
        var databaseName = vaultClient.GetSecret(databaseSecretName).Value.Value;

        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));

        services.AddSingleton<IMongoClient>(s =>
        {
            var settings = s.GetRequiredService<IOptions<MongoDbSettings>>().Value;

            var clientSettings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            clientSettings.UseTls = true;
            clientSettings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
            clientSettings.ConnectTimeout = TimeSpan.FromSeconds(settings.Timeouts.ConnectionTimeoutSeconds);
            clientSettings.SocketTimeout = TimeSpan.FromSeconds(settings.Timeouts.SocketTimeoutSeconds);

            return new MongoClient(clientSettings);
        });

        services.AddScoped<IMongoDatabase>(s =>
        {
            var client = s.GetRequiredService<IMongoClient>();

            if (string.IsNullOrEmpty(databaseName))
                throw new InvalidOperationException($"Environment variable '{databaseName}' is missing or empty.");

            return client.GetDatabase(databaseName);
        });

        services.AddScoped<IMongoDbSettings>(s =>
        {
            return s.GetRequiredService<IOptions<MongoDbSettings>>().Value;
        });

        services.AddSingleton<IPolicyProvider>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<PolicyProvider>>();
            return new PolicyProvider(logger);
        });

        services.AddScoped<IRepository<AccountDocument>>(provider =>
        {
            var settings = provider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            var mongoDatabase = provider.GetRequiredService<IMongoDatabase>();
            var accountsCollection = settings.Collections.Accounts;
            var logger = provider.GetRequiredService<ILogger<AccountsRepository<AccountDocument>>>();
            var policyProvider = provider.GetRequiredService<IPolicyProvider>();

            return new AccountsRepository<AccountDocument>(mongoDatabase, accountsCollection, logger, policyProvider);
        });

        services.AddScoped<IAccountsFilterHandler, NabpFilterHandler>();
        services.AddScoped<IAccountsFilterHandler, NpiFilterHandler>();
        services.AddScoped<IAccountsFilterHandler, GroupFilterHandler>();
        services.AddScoped<IAccountsFilterHandler, BinIinAndBillCodeFilterHandler>();
        services.AddScoped<IFilterHandlerFactory, FilterHandlerFactory>();
        services.AddSingleton<IPolicyProvider, PolicyProvider>();

        services.AddTransient<IValidator<AccountsRequestDto>, AccountsRequestDtoValidator>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetSearchAccountsQueryHandler).Assembly));

        return services;
    }
}
namespace Aps.Configuration.Apis;

public static class ConfigureServices
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));

        services.AddSingleton<IMongoClient>(s =>
        {
            var settings = s.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            var connectionString = Environment.GetEnvironmentVariable(settings.ConnectionStringSecretName);

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException($"Environment variable '{settings.ConnectionStringSecretName}' is missing or empty.");

            X509Certificate2 certificate;
            try
            {
                certificate = new X509Certificate2(settings.Certificate.PathSecretName);
            }
            catch (CryptographicException ex)
            {
                throw new InvalidOperationException("Failed to load the certificate", ex);
            }

            var clientSettings = MongoClientSettings.FromConnectionString(connectionString);

            clientSettings.UseTls = true;
            clientSettings.SslSettings = new SslSettings
            {
                ClientCertificates = new[] { certificate },
                CheckCertificateRevocation = false
            };
            clientSettings.ServerApi = new ServerApi(ServerApiVersion.V1);
            clientSettings.ConnectTimeout = TimeSpan.FromSeconds(settings.Timeouts.ConnectionTimeoutSeconds);
            clientSettings.SocketTimeout = TimeSpan.FromSeconds(settings.Timeouts.SocketTimeoutSeconds);

            return new MongoClient(clientSettings);
        });

        services.AddScoped<IMongoDatabase>(s =>
        {
            var settings = s.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            var client = s.GetRequiredService<IMongoClient>();

            var databaseName = Environment.GetEnvironmentVariable(settings.DatabaseSecretName);

            if (string.IsNullOrEmpty(databaseName))
                throw new InvalidOperationException($"Environment variable '{settings.DatabaseSecretName}' is missing or empty.");

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
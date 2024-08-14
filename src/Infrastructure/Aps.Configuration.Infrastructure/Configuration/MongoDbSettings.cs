namespace Aps.Configuration.Infrastructure.Configuration;

public class MongoDbSettings : IMongoDbSettings
{
    public string ConnectionStringSecretName { get; set; }
    public string DatabaseSecretName { get; set; }
    public CertificateSettings Certificate { get; set; }
    public TimeoutSettings Timeouts { get; set; }
    public CollectionSettings Collections { get; set; }
}

public class CertificateSettings
{
    public string PathSecretName { get; set; }
}

public class TimeoutSettings
{
    public int ConnectionTimeoutSeconds { get; set; }
    public int SocketTimeoutSeconds { get; set; }
}

public class CollectionSettings
{
    public string Accounts { get; set; }
}
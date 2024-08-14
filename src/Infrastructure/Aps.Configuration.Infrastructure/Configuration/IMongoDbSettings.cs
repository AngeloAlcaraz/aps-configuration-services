namespace Aps.Configuration.Infrastructure.Configuration;

public interface IMongoDbSettings
{
    public string ConnectionStringSecretName { get; set; }
    public string DatabaseSecretName { get; set; }
    public CertificateSettings Certificate { get; set; }
    public TimeoutSettings Timeouts { get; set; }
    public CollectionSettings Collections { get; set; }
}
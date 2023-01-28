using Microsoft.Extensions.Configuration;

namespace Sestio.Usuarios.IntegrationTests.Base.Helpers;

public sealed class DatabaseConnectionSettings
{
    private string? _host;
    private int? _port;
    private string? _username;
    private string? _password;
    private string? _database;

    private DatabaseConnectionSettings()
    {
    }

    public string ConnectionString => $"Host={_host};Port={_port};Database=\"{_database}\";Username={_username};Password={_password}";

    public static DatabaseConnectionSettings FromConfiguration(IConfiguration configuration)
    {
        return new DatabaseConnectionSettings
        {
            _host = configuration.GetValue<string>("DB_HOST", "")!,
            _port = configuration.GetValue<int>("DB_PORT", 0)!,
            _database = configuration.GetValue<string>("DB_NAME", "")!,
            _username = configuration.GetValue<string>("DB_USERNAME", "")!,
            _password = configuration.GetValue<string>("DB_PASSWORD", "")!
        };
    }

    public DatabaseConnectionSettings WithRandomDbName(string namePrefix)
    {
        return new DatabaseConnectionSettings
        {
            _host = _host,
            _port = _port,
            _username = _username,
            _password = _password,
            _database = $"{namePrefix}_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid():N}"
        };
    }
}

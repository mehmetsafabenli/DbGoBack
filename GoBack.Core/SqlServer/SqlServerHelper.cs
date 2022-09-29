using Microsoft.Extensions.Configuration;

namespace GoBack.Core.SqlServer;

public class SqlServerHelper
{
    private readonly string _connectionString;
    private readonly IConfiguration _configuration;

    public SqlServerHelper(string connectionString,
        IConfiguration configuration)
    {
        _connectionString = connectionString;
        _configuration = configuration;
    }

    #region GetConnectionString

    private string GetConnectionString(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            if (IsConnectionStringValid(connectionString))
            {
            }
        }

        int a = 10_000;

        return connectionString;
    }

    private bool IsConnectionStringValid(string connectionString)
    {
        return !string.IsNullOrEmpty(connectionString)
               && connectionString.Length > 0
               && connectionString.Contains(';');
    }

    private bool ConfigurationHaveConnectionString()
    {
        return _configuration.GetConnectionString("GoBackDbConnection")
               != null;
    }

    #endregion
}
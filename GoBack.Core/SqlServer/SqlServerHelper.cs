using System.Data.Common;
using GoBack.Core.SqlServer.Options;
using Microsoft.Extensions.Configuration;

namespace GoBack.Core.SqlServer;

public class SqlServerHelper
{
    private readonly string _connectionString;
    private readonly Func<DbConnection> _connectionFactory;
    private readonly SqlServerOptions _options;
    private readonly IConfiguration _configuration;

    public SqlServerHelper(string connectionString,
        IConfiguration configuration, Func<DbConnection> connectionFactory, SqlServerOptions options)
    {
        _connectionString = connectionString;
        _configuration = configuration;
        _connectionFactory = connectionFactory;
        _options = options;
    }

    #region GetConnectionString

    private string GetConnectionString(string connectionString)
    {
        if (IsConnectionStringValid(connectionString))
        {
            return connectionString;
        }

        if (ConfigurationHaveConnectionString(connectionString))
        {
            return _configuration.GetConnectionString(connectionString);
        }

        throw new ArgumentException("Connection string is not valid");
    }

    private bool IsConnectionStringValid(string connectionString)
    {
        return !string.IsNullOrEmpty(connectionString)
               && connectionString.Length > 0
               && connectionString.Contains(';');
    }

    private bool ConfigurationHaveConnectionString(string connectionString)
    {
        return _configuration.GetConnectionString(connectionString)
               != null;
    }

    internal DbConnection CreateAndOpenConnection()
    {
        using (_options.Dispose?.Invoke())
        {
            return _connectionFactory();
        }
    }

    #endregion
}
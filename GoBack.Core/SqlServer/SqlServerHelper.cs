using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using GoBack.Core.SqlServer.Options;
using GoBack.Core.System;
using Microsoft.Extensions.Configuration;

namespace GoBack.Core.SqlServer;

public class SqlServerHelper
{
    private readonly string _connectionString;
    private readonly DbConnection _existingConnection;
    private readonly Func<DbConnection> _connectionFactory;
    private readonly SqlServerOptions _options;
    private readonly IConfiguration _configuration;

    public SqlServerHelper(string connectionString,
        IConfiguration configuration, Func<DbConnection> connectionFactory, SqlServerOptions options,
        DbConnection existingConnection)
    {
        _connectionString = connectionString;
        _configuration = configuration;
        _connectionFactory = connectionFactory;
        _options = options;
        _existingConnection = existingConnection;
    }

    #region GetConnectionString

    internal void UseTransaction(DbConnection dedicatedConnection,
        Action<DbConnection, DbTransaction> action)
    {
        UseTransaction(dedicatedConnection, (connection, transaction) =>
        {
            action(connection, transaction);
            return true;
        }, null);
    }

    private T UseTransaction<T>(
        DbConnection dedicatedConnection, Func<DbConnection, DbTransaction, T> func,
        IsolationLevel? isolationLevel)
    {
        var connection = dedicatedConnection;
        var transaction = connection.BeginTransaction(isolationLevel ?? IsolationLevel.ReadCommitted);
        try
        {
            var result = func(connection, transaction);
            transaction.Commit();
            return result;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
        finally
        {
            connection.Dispose();
        }
    }

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
            DbConnection connection = null;
            try
            {
                connection = _existingConnection ?? _connectionFactory();
                if (connection.State != ConnectionState.Closed)
                    connection.Open();

                return connection;
            }
            catch (Exception ex)
            {
                connection?.Dispose();
                throw;
            }
        }
    }

    internal void ReleaseConnection(IDbConnection? connection)
    {
        if (connection != null && !IsExistingConnection(connection))
        {
            connection.Dispose();
        }

        _options.Dispose?.Invoke();
    }

    private bool IsExistingConnection(IDbConnection? connection)
    {
        return connection != null && ReferenceEquals(connection, _existingConnection);
    }

    internal void UseConnection(DbConnection dedicatedConnection, Action<DbConnection> action)
    {
        UseConnection(dedicatedConnection, connection =>
        {
            action(connection);
            return true;
        });
    }

    private T UseConnection<T>(DbConnection dedicatedConnection, Func<DbConnection, T> func)
    {
        DbConnection connection = null;

        try
        {
            connection = dedicatedConnection ?? CreateAndOpenConnection();
            return func(connection);
        }
        finally
        {
            if (dedicatedConnection == null)
            {
                ReleaseConnection(connection);
            }
        }
    }

    #endregion
}
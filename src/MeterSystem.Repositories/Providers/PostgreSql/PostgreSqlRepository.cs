using MeterSystem.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Npgsql;

public abstract class PostgreSqlRepository(IConfiguration configuration) : IRepository
{
    protected abstract string ConnectionStringName { get; }
    public async Task SaveAsync<T>(T entity, CancellationToken cancellationToken = default)
    {
        string connectionString = configuration.GetConnectionString(ConnectionStringName)
            ?? throw new InvalidOperationException($"Connection string '{ConnectionStringName}' is missing.");

        await using NpgsqlConnection connection = new(connectionString);

        await connection.OpenAsync(cancellationToken);

        await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            await SaveInternalAsync(connection, transaction, entity, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    protected abstract Task SaveInternalAsync<T>(NpgsqlConnection connection, NpgsqlTransaction transaction, T entity, CancellationToken cancellationToken);
}
using Dapper;
using MeterSystem.Shared.Models;
using Npgsql;

namespace MeterSystem.Worker.Repositories;

internal class MeterReadingsRepository(IConfiguration configuration) : PostgreSqlRepository(configuration)
{
    protected override string ConnectionStringName => "Postgres";

    protected override async Task SaveInternalAsync<T>(NpgsqlConnection connection, NpgsqlTransaction transaction, T entity, CancellationToken cancellationToken)
    {
        MeterData request = entity as MeterData
            ?? throw new NotSupportedException($"Type '{typeof(T).Name}' is not supported.");

        int meterId = await GetOrCreateMeterAsync(connection, transaction, request.MeterNumber);

        await InsertReadingsAsync(connection, transaction, meterId, request.Readings);
    }

    private async Task<int> GetOrCreateMeterAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, long meterNumber)
    {
        const string sql = """
            INSERT INTO meters (meter_number)
            VALUES (@MeterNumber)
            ON CONFLICT (meter_number)
            DO UPDATE SET meter_number = EXCLUDED.meter_number
            RETURNING meter_id;
            """;

        return await connection.ExecuteScalarAsync<int>(sql, new { MeterNumber = meterNumber }, transaction);
    }

    private async Task InsertReadingsAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, int meterId, IReadOnlyDictionary<DateTime, double> readings)
    {
        const string sql = """
            INSERT INTO meter_readings
            (
                meter_id,
                value_at,
                value
            )
            VALUES
            (
                @MeterId,
                @ValueAt,
                @Value
            )
            ON CONFLICT (meter_id, value_at)
            DO NOTHING;
            """;

        foreach (KeyValuePair<DateTime, double> reading in readings)
        {
            await connection.ExecuteAsync(
                sql,
                new
                {
                    MeterId = meterId,
                    ValueAt = reading.Key,
                    Value = reading.Value
                },
                transaction);
        }
    }
}
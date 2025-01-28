using Npgsql;
using PassengerService.Application.Abstractions.Persistence.Queries;
using PassengerService.Application.Abstractions.Persistence.Repositories;
using PassengerService.Application.Models.Passengers;
using System.Runtime.CompilerServices;

namespace PassengerService.Infrastructure.Persistence.Repositories;

public class PassengerRepository : IPassengerRepository
{
    private readonly NpgsqlDataSource _npgsqlDataSource;

    public PassengerRepository(NpgsqlDataSource dataSource)
    {
        _npgsqlDataSource = dataSource;
    }

    public async Task CreatePassenger(CreatePassengerQuery query, CancellationToken cancellationToken)
    {
        const string sql = """
                           INSERT INTO passengers (passenger_passport, passenger_name, passenger_email, passenger_birthday, passenger_banned)
                           VALUES (@Passport, @Name, @Email, @Birthday, false)
                           RETURNING passenger_id;
                           """;

        await using NpgsqlConnection connection = await _npgsqlDataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("Passport", query.Passport),
                new NpgsqlParameter("Name", query.Name),
                new NpgsqlParameter("Email", query.Email),
                new NpgsqlParameter("Birthday", query.Birthday),
            },
        };

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task BanPassenger(long id, CancellationToken cancellationToken)
    {
        const string sql = """
                           UPDATE passengers
                           SET passenger_banned = true
                           WHERE passenger_id = @PassengerId;
                           """;

        await using NpgsqlConnection connection = await _npgsqlDataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("PassengerId", id),
            },
        };
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<Passenger?> GetPassengerById(long id, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT passenger_id,
                                  passenger_passport,
                                  passenger_name,
                                  passenger_email,
                                  passenger_birthday,
                                  passenger_banned
                           FROM passengers
                           WHERE passenger_id = @Id;
                           """;

        await using NpgsqlConnection connection = await _npgsqlDataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("Id", id),
            },
        };

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return new Passenger(
            reader.GetInt64(reader.GetOrdinal("passenger_id")),
            reader.GetInt64(reader.GetOrdinal("passenger_passport")),
            reader.GetString(reader.GetOrdinal("passenger_name")),
            reader.GetString(reader.GetOrdinal("passenger_email")),
            reader.GetDateTime(reader.GetOrdinal("passenger_birthday")),
            reader.GetBoolean(reader.GetOrdinal("passenger_banned")));
    }

    public async IAsyncEnumerable<Passenger> GetPassengersAsync(
        GetPassengersQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT  passenger_id,
                                   passenger_passport,
                                   passenger_name,
                                   passenger_email,
                                   passenger_birthday,
                                   passenger_banned
                           FROM passengers
                           WHERE (passenger_id > @Cursor)
                           AND (@Name IS NULL OR passenger_name LIKE @Name)
                           AND (cardinality(@Ids) = 0 OR passenger_id = any(@Ids))
                           AND (cardinality(@PassportIds) = 0 OR passenger_passport = any(@PassportIds))
                           AND (cardinality(@Emails) = 0 OR passenger_email = any(@Emails))
                           LIMIT @Limit;
                           """;

        await using NpgsqlConnection connection = await _npgsqlDataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter<string?>("Name", query.NameSubstring),
                new NpgsqlParameter("Cursor", query.Cursor),
                new NpgsqlParameter("Limit", query.PageSize),
                new NpgsqlParameter("Ids", query.Ids),
                new NpgsqlParameter("PassportIds", query.PassportIds),
                new NpgsqlParameter("Emails", query.Emails),
            },
        };

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new Passenger(
                reader.GetInt64(reader.GetOrdinal("passenger_id")),
                reader.GetInt64(reader.GetOrdinal("passenger_passport")),
                reader.GetString(reader.GetOrdinal("passenger_name")),
                reader.GetString(reader.GetOrdinal("passenger_email")),
                reader.GetDateTime(reader.GetOrdinal("passenger_birthday")),
                reader.GetBoolean(reader.GetOrdinal("passenger_banned")));
        }
    }
}
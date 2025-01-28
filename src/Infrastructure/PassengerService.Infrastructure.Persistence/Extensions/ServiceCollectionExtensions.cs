using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using PassengerService.Application.Abstractions.Persistence.Repositories;
using PassengerService.Infrastructure.Persistence.Migrations;
using PassengerService.Infrastructure.Persistence.Options;
using PassengerService.Infrastructure.Persistence.Repositories;

namespace PassengerService.Infrastructure.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMigration(
        this IServiceCollection collection)
    {
        collection
            .AddFluentMigratorCore()
            .ConfigureRunner(runner => runner
                .AddPostgres()
                .WithGlobalConnectionString(serviceProvider =>
                    serviceProvider
                        .GetRequiredService<IOptions<PostgresOptions>>()
                        .Value
                        .GetConnectionString())
                .ScanIn(typeof(InitialMigration).Assembly));

        collection.AddSingleton<NpgsqlDataSource>(provider =>
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(provider
                .GetRequiredService<IOptions<PostgresOptions>>()
                .Value
                .GetConnectionString());

            return dataSourceBuilder.Build();
        });

        return collection;
    }

    public static IServiceCollection AddInfrastructureDataAccess(this IServiceCollection collection)
    {
        collection.AddScoped<IPassengerRepository, PassengerRepository>();

        return collection;
    }
}
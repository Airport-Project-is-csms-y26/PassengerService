using FluentMigrator;

namespace PassengerService.Infrastructure.Persistence.Migrations;

[Migration(1, "Initial")]
public class InitialMigration : Migration
{
    public override void Up()
    {
        string sql = """
        CREATE TABLE Passengers (
             passenger_id BIGINT primary key generated always as identity,
             passenger_passport BIGINT NOT NULL UNIQUE,
             passenger_name VARCHAR(255) NOT NULL,
             passenger_email VARCHAR(255) NOT NULL UNIQUE,
             passenger_birthday TIMESTAMP WITH TIME ZONE NOT NULL,
             passenger_banned boolean not null
         );
        """;
        Execute.Sql(sql);
    }

    public override void Down()
    {
        Execute.Sql("""
        DROP TABLE Passengers;
        """);
    }
}
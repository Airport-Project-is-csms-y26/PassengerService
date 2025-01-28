namespace PassengerService.Application.Abstractions.Persistence.Queries;

public record CreatePassengerQuery(string Name, long Passport, string Email, DateTimeOffset Birthday);
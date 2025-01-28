namespace PassengerService.Application.Models.Passengers;

public record Passenger(long Id, long Passport, string Name, string Email, DateTimeOffset Birthday, bool IsBanned);
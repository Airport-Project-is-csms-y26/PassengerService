namespace PassengerService.Application.Contracts.Passengers.Operations;

public record GetPassengersRequest(
    int PageSize,
    int Cursor,
    long[] Ids,
    long[] PassportIds,
    string[] Emails,
    string? NameSubstring);
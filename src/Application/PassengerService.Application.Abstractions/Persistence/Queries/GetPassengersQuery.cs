namespace PassengerService.Application.Abstractions.Persistence.Queries;

public record GetPassengersQuery(
    int PageSize,
    int Cursor,
    long[] Ids,
    long[] PassportIds,
    string[] Emails,
    string? NameSubstring);
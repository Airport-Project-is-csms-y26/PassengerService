using PassengerService.Presentation.TicketGrpcClient.Clients.Interfaces;
using System.Runtime.CompilerServices;
using Tickets.TicketsService.Contracts;

namespace PassengerService.Presentation.TicketGrpcClient.Clients;

public class TicketClient : ITicketClient
{
    private readonly TicketsService.TicketsServiceClient _client;

    public TicketClient(TicketsService.TicketsServiceClient client)
    {
        _client = client;
    }

    public async Task<CreateTicketResponse> CreateTicket(
        long passengerId,
        long flightId,
        long place,
        CancellationToken cancellationToken)
    {
        var request = new CreateTicketRequest
        {
            PassengerId = passengerId,
            FlightId = flightId,
            Place = place,
        };

        return await _client.CreateAsync(request, cancellationToken: cancellationToken);
    }

    public async Task<RegisterPassengerOnFlightResponse> RegisterPassengerOnFlight(
        long ticketId,
        CancellationToken cancellationToken)
    {
        var request = new RegisterPassengerOnFlightRequest
        {
            TicketId = ticketId,
        };

        return await _client.RegisterPassengerOnFlightAsync(request, cancellationToken: cancellationToken);
    }

    public async IAsyncEnumerable<Ticket> GetTickets(
        int pageSize,
        int cursor,
        long[] ids,
        long[] flightIds,
        long[] passengerIds,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var request = new GetTicketsRequest
        {
            Cursor = cursor,
            PageSize = pageSize,
            TicketIds = { ids },
            FlightIds = { flightIds },
            PassengerIds = { passengerIds },
        };

        GetTicketsResponse response =
            await _client.GetTicketsAsync(request, cancellationToken: cancellationToken);

        foreach (Ticket passenger in response.Tickets)
        {
            yield return passenger;
        }
    }
}
using Tickets.TicketsService.Contracts;

namespace PassengerService.Presentation.TicketGrpcClient.Clients.Interfaces;

public interface ITicketClient
{
    Task<CreateTicketResponse> CreateTicket(long passengerId, long flightId, long place, CancellationToken cancellationToken);

    Task<RegisterPassengerOnFlightResponse> RegisterPassengerOnFlight(long ticketId, CancellationToken cancellationToken);

    IAsyncEnumerable<Ticket> GetTickets(int pageSize, int cursor, long[] ids, long[] flightIds, long[] passengerIds, CancellationToken cancellationToken);
}
using PassengerService.Application.Abstractions.Persistence.Queries;
using PassengerService.Application.Abstractions.Persistence.Repositories;
using PassengerService.Application.Contracts.Notifications;
using PassengerService.Application.Contracts.Passengers;
using PassengerService.Application.Contracts.Passengers.Operations;
using PassengerService.Application.Models.Passengers;
using PassengerService.Presentation.TicketGrpcClient.Clients.Interfaces;
using Tickets.TicketsService.Contracts;

namespace PassengerService.Application.Passengers;

public class MyPassengerService : IPassengerService
{
    private readonly IPassengerRepository _passengerRepository;
    private readonly INotificationService _notificationService;
    private readonly ITicketClient _ticketClient;

    public MyPassengerService(
        IPassengerRepository passengerRepository,
        INotificationService notificationService,
        ITicketClient ticketClient)
    {
        _passengerRepository = passengerRepository;
        _notificationService = notificationService;
        _ticketClient = ticketClient;
    }

    public async Task<CreatePassenger.Result> CreateAsync(
        CreatePassenger.Request request,
        CancellationToken cancellationToken)
    {
        var query = new GetPassengersQuery(
            int.MaxValue,
            0,
            Array.Empty<long>(),
            new[] { request.Passport },
            Array.Empty<string>(),
            null);
        Passenger[] samePassportPassengers = await _passengerRepository
            .GetPassengersAsync(query, cancellationToken)
            .ToArrayAsync(cancellationToken);

        if (samePassportPassengers.Length != 0)
        {
            return new CreatePassenger.Result.InvalidData();
        }

        await _passengerRepository.CreatePassenger(
            new CreatePassengerQuery(request.Name, request.Passport, request.Email, request.Birthday),
            cancellationToken);

        return new CreatePassenger.Result.Success();
    }

    public async Task<BanPassenger.Result> BanAsync(BanPassenger.Request request, CancellationToken cancellationToken)
    {
        Passenger? passenger = await _passengerRepository.GetPassengerById(request.Id, cancellationToken);

        if (passenger == null)
        {
            return new BanPassenger.Result.PassengerNotFound();
        }

        await _passengerRepository.BanPassenger(request.Id, cancellationToken);

        return new BanPassenger.Result.Success();
    }

    public IAsyncEnumerable<Passenger> GetPassengers(GetPassengersRequest request, CancellationToken cancellationToken)
    {
        var query = new GetPassengersQuery(
            request.PageSize,
            request.Cursor,
            request.Ids,
            request.PassportIds,
            request.Emails,
            request.NameSubstring);

        return _passengerRepository.GetPassengersAsync(query, cancellationToken);
    }

    public async Task NotifyPassengers(
        long flightId,
        string message,
        DateTimeOffset time,
        CancellationToken cancellationToken)
    {
        Ticket[] tickets = await _ticketClient.GetTickets(
                int.MaxValue,
                0,
                Array.Empty<long>(),
                new[] { flightId },
                Array.Empty<long>(),
                cancellationToken)
            .ToArrayAsync(cancellationToken);

        long[] passengerIds = tickets.Select(ticket => ticket.TicketPassengerId).ToArray();
        var query = new GetPassengersQuery(
            int.MaxValue,
            0,
            passengerIds,
            Array.Empty<long>(),
            Array.Empty<string>(),
            null);

        await foreach (Passenger passenger in _passengerRepository.GetPassengersAsync(query, cancellationToken))
        {
            _notificationService.Notify(passenger.Email, time + " " + message);
        }
    }
}
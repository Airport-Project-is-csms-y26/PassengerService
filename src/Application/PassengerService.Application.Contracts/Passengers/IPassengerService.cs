using PassengerService.Application.Contracts.Passengers.Operations;
using PassengerService.Application.Models.Passengers;

namespace PassengerService.Application.Contracts.Passengers;

public interface IPassengerService
{
    Task<CreatePassenger.Result> CreateAsync(CreatePassenger.Request request, CancellationToken cancellationToken);

    Task<BanPassenger.Result> BanAsync(BanPassenger.Request request, CancellationToken cancellationToken);

    IAsyncEnumerable<Passenger> GetPassengers(GetPassengersRequest request, CancellationToken cancellationToken);

    Task NotifyPassengers(long flightId, string message, DateTimeOffset time, CancellationToken cancellationToken);
}
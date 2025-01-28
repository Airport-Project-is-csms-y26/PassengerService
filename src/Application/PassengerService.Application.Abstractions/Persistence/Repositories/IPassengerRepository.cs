using PassengerService.Application.Abstractions.Persistence.Queries;
using PassengerService.Application.Models.Passengers;

namespace PassengerService.Application.Abstractions.Persistence.Repositories;

public interface IPassengerRepository
{
    Task CreatePassenger(CreatePassengerQuery query, CancellationToken cancellationToken);

    Task BanPassenger(long id, CancellationToken cancellationToken);

    Task<Passenger?> GetPassengerById(long id, CancellationToken cancellationToken);

    IAsyncEnumerable<Passenger> GetPassengersAsync(GetPassengersQuery query, CancellationToken cancellationToken);
}
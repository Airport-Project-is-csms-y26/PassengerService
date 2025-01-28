using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Passengers.PassengerService.Contracts;
using PassengerService.Application.Contracts.Passengers;
using PassengerService.Application.Contracts.Passengers.Operations;
using System.Diagnostics;
using GetPassengersRequest = Passengers.PassengerService.Contracts.GetPassengersRequest;

namespace PassengerService.Presentation.Grpc.Controllers;

public class PassengerController : Passengers.PassengerService.Contracts.PassengerService.PassengerServiceBase
{
    private readonly IPassengerService _passengerService;

    public PassengerController(IPassengerService passengerService)
    {
        _passengerService = passengerService;
    }

    public override async Task<CreatePassengerResponse> Create(
        CreatePassengerRequest request,
        ServerCallContext context)
    {
        CreatePassenger.Result result = await _passengerService.CreateAsync(
            new CreatePassenger.Request(
                request.Name,
                request.Passport,
                request.Email,
                request.Birthday.ToDateTimeOffset()),
            context.CancellationToken);

        return result switch
        {
            CreatePassenger.Result.Success => new CreatePassengerResponse(),

            CreatePassenger.Result.InvalidData => throw new RpcException(new Status(
                StatusCode.InvalidArgument,
                "Invalid data: this passport already exists")),

            _ => throw new UnreachableException(),
        };
    }

    public override async Task<BanPassengerResponse> Ban(BanPassengerRequest request, ServerCallContext context)
    {
        BanPassenger.Result result =
            await _passengerService.BanAsync(new BanPassenger.Request(request.Id), context.CancellationToken);

        return result switch
        {
            BanPassenger.Result.Success => new BanPassengerResponse(),

            BanPassenger.Result.PassengerNotFound => throw new RpcException(new Status(
                StatusCode.NotFound,
                "PassengerNotFound")),

            _ => throw new UnreachableException(),
        };
    }

    public override async Task<GetPassengersResponse> GetPassengers(
        GetPassengersRequest request,
        ServerCallContext context)
    {
        var query = new Application.Contracts.Passengers.Operations.GetPassengersRequest(
            request.PageSize,
            request.Cursor,
            request.PassengerIds.ToArray(),
            request.PassportIds.ToArray(),
            request.Emails.ToArray(),
            request.Name);

        IAsyncEnumerable<Application.Models.Passengers.Passenger> passengers =
            _passengerService.GetPassengers(query, context.CancellationToken);

        var reply = new GetPassengersResponse();
        await foreach (Application.Models.Passengers.Passenger passenger in passengers)
        {
            reply.Passengers.Add(new Passenger
            {
                PassengerId = passenger.Id,
                PassengerPassport = passenger.Passport,
                Name = passenger.Name,
                Email = passenger.Email,
                Birthday = Timestamp.FromDateTime(passenger.Birthday.UtcDateTime),
                IsBanned = passenger.IsBanned,
            });
        }

        return reply;
    }
}
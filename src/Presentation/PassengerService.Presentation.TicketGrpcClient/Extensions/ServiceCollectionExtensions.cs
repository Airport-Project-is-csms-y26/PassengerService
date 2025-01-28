using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PassengerService.Presentation.TicketGrpcClient.Clients;
using PassengerService.Presentation.TicketGrpcClient.Clients.Interfaces;
using PassengerService.Presentation.TicketGrpcClient.Options;
using Tickets.TicketsService.Contracts;

namespace PassengerService.Presentation.TicketGrpcClient.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTicketGrpcClient(
        this IServiceCollection collection)
    {
        collection.AddGrpcClient<TicketsService.TicketsServiceClient>((sp, o) =>
        {
            IOptions<TicketServiceClientOptions> options = sp.GetRequiredService<IOptions<TicketServiceClientOptions>>();
            o.Address = new Uri(options.Value.GrpcServerUrl);
        });

        collection.AddScoped<ITicketClient, TicketClient>();
        return collection;
    }
}
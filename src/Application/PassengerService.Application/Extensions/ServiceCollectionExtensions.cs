using Microsoft.Extensions.DependencyInjection;
using PassengerService.Application.Contracts.Notifications;
using PassengerService.Application.Contracts.Passengers;
using PassengerService.Application.Notifications;
using PassengerService.Application.Passengers;

namespace PassengerService.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection collection)
    {
        collection.AddScoped<INotificationService, ConsoleNotificator>();

        collection.AddScoped<IPassengerService, MyPassengerService>();
        return collection;
    }
}
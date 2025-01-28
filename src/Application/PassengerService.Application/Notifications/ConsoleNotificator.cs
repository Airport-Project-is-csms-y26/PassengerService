using PassengerService.Application.Contracts.Notifications;

namespace PassengerService.Application.Notifications;

public class ConsoleNotificator : INotificationService
{
    public void Notify(string email, string message)
    {
        Console.WriteLine(email);
        Console.WriteLine(message);
    }
}
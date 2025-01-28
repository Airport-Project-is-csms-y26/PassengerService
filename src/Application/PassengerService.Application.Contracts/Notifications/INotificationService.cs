namespace PassengerService.Application.Contracts.Notifications;

public interface INotificationService
{
    void Notify(string email, string message);
}
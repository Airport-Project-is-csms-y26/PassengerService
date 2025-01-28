using PassengerService.Application.Contracts.Passengers;
using Tasks.Kafka.Contracts;

namespace PassengerService.Presentation.Kafka.Consumer.Handlers;

public class NotificationsHandler : IKafkaMessageHandler<PassengerNotificationsKey, PassengerNotificationsValue>
{
    private readonly IPassengerService _passengerService;

    public NotificationsHandler(IPassengerService passengerService)
    {
        _passengerService = passengerService;
    }

    public async Task HandleAsync(IEnumerable<IKafkaConsumerMessage<PassengerNotificationsKey, PassengerNotificationsValue>> messages, CancellationToken cancellationToken)
    {
        foreach (IKafkaConsumerMessage<PassengerNotificationsKey, PassengerNotificationsValue> message in messages)
        {
            await _passengerService.NotifyPassengers(
                message.Value.FlightId,
                message.Value.Message,
                message.Value.MessageTime.ToDateTimeOffset(),
                cancellationToken);
        }
    }
}
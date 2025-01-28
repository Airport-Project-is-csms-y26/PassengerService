using Confluent.Kafka;
using Google.Protobuf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PassengerService.Presentation.Kafka.Consumer;
using PassengerService.Presentation.Kafka.Consumer.Handlers;
using PassengerService.Presentation.Kafka.Consumer.Services;
using PassengerService.Presentation.Kafka.Producer;
using PassengerService.Presentation.Kafka.Tools;
using Tasks.Kafka.Contracts;

namespace PassengerService.Presentation.Kafka.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHandlers(this IServiceCollection collection)
    {
        collection.AddScoped<IKafkaMessageHandler<PassengerNotificationsKey,
            PassengerNotificationsValue>, NotificationsHandler>();
        return collection;
    }

    public static IServiceCollection AddKafkaConsumer<TKey, TValue>(
        this IServiceCollection collection,
        string optionsKey) where TKey : IMessage<TKey>, new()
        where TValue : IMessage<TValue>, new()
    {
        collection.AddKeyedSingleton<IDeserializer<TKey>, ProtobufSerializer<TKey>>(
            optionsKey);
        collection.AddKeyedSingleton<IDeserializer<TValue>, ProtobufSerializer<TValue>>(
            optionsKey);
        collection.AddHostedService(p
            => ActivatorUtilities.CreateInstance<MyBatchingKafkaConsumerService<TKey, TValue>>(p, optionsKey));
        return collection;
    }

    public static IServiceCollection AddKafkaProducer<TKey, TValue>(
        this IServiceCollection collection,
        string optionsKey) where TKey : IMessage<TKey>, new()
        where TValue : IMessage<TValue>, new()
    {
        collection.AddKeyedSingleton<ISerializer<TKey>, ProtobufSerializer<TKey>>(optionsKey);
        collection.AddKeyedSingleton<ISerializer<TValue>, ProtobufSerializer<TValue>>(optionsKey);
        collection.TryAddScoped<IKafkaProducer<TKey, TValue>>(
            p => ActivatorUtilities
                .CreateInstance<KafkaProducer<TKey, TValue>>(
                    p,
                    optionsKey));

        return collection;
    }
}
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using SmartCafe.Menu.Application.Interfaces;

namespace SmartCafe.Menu.Infrastructure.EventBus;

public class ServiceBusEventPublisher(ServiceBusClient serviceBusClient, string topicName) : IEventPublisher
{
    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : class
    {
        await using var sender = serviceBusClient.CreateSender(topicName);

        var messageBody = JsonSerializer.Serialize(@event);
        var message = new ServiceBusMessage(messageBody)
        {
            ContentType = "application/json",
            Subject = typeof(TEvent).Name
        };

        await sender.SendMessageAsync(message, cancellationToken);
    }
}

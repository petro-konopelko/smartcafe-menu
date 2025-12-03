using System.Text.Json;
using Azure.Messaging.ServiceBus;
using SmartCafe.Menu.Application.Interfaces;

namespace SmartCafe.Menu.Infrastructure.EventBus;

public class ServiceBusEventPublisher(ServiceBusClient serviceBusClient, string topicName) : IEventPublisher
{
    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : class
    {
        ArgumentNullException.ThrowIfNull(@event);

        await SendAsync(@event, typeof(TEvent).Name, cancellationToken);
    }

    public async Task PublishAsync(object @event, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@event);

        await SendAsync(@event, @event.GetType().Name, cancellationToken);
    }

    private async Task SendAsync(object @event, string subject, CancellationToken cancellationToken)
    {
        await using var sender = serviceBusClient.CreateSender(topicName);

        var messageBody = JsonSerializer.Serialize(@event);
        var message = new ServiceBusMessage(messageBody)
        {
            ContentType = "application/json",
            Subject = subject
        };

        // Add additional metadata for event tracing and debugging
        message.ApplicationProperties["eventId"] = Guid.NewGuid().ToString();
        message.ApplicationProperties["publishedAt"] = DateTimeOffset.UtcNow.ToString("O");
        message.ApplicationProperties["source"] = "SmartCafe.Menu.API";

        await sender.SendMessageAsync(message, cancellationToken);
    }
}

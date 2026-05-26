using System.Text.Json;
using MeterSystem.Messaging.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MeterSystem.Messaging.Providers.RabbitMq;

public class RabbitMqMessageConsumer(IOptions<RabbitMqOptions> options) : IMessageConsumer
{
    public async Task ReceivedAsync<T>(Func<T, CancellationToken, Task> handleMessageAsync, CancellationToken cancellationToken = default)
    {
        RabbitMqOptions rabbitMqOptions = options.Value;
        ConnectionFactory factory = new()
        {
            HostName = rabbitMqOptions.Host,
            Port = rabbitMqOptions.Port,
            UserName = rabbitMqOptions.Username,
            Password = rabbitMqOptions.Password
        };

        IConnection connection = await factory.CreateConnectionAsync(cancellationToken);
        IChannel channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue: rabbitMqOptions.QueueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        AsyncEventingBasicConsumer consumer = new(channel);

        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            T? message = JsonSerializer.Deserialize<T>(eventArgs.Body.Span);

            if (message is null)
            {
                await channel.BasicNackAsync(
                    eventArgs.DeliveryTag,
                    multiple: false,
                    requeue: false,
                    cancellationToken: cancellationToken);
            }
            else
            {
                await handleMessageAsync(message, cancellationToken);

                await channel.BasicAckAsync(
                    eventArgs.DeliveryTag,
                    multiple: false,
                    cancellationToken: cancellationToken);
            }
        };

        await channel.BasicConsumeAsync(
            queue: rabbitMqOptions.QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken);
    }
}
using System.Text.Json;
using MeterSystem.Messaging.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace MeterSystem.Messaging.Providers.RabbitMq;

public class RabbitMqMessageProducer(IOptions<RabbitMqOptions> options) : IMessageProducer
{
    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
    {
        RabbitMqOptions rabbitMqOptions = options.Value;
        ConnectionFactory factory = new()
        {
            HostName = rabbitMqOptions.Host,
            Port = rabbitMqOptions.Port,
            UserName = rabbitMqOptions.Username,
            Password = rabbitMqOptions.Password
        };

        await using IConnection connection = await factory.CreateConnectionAsync(cancellationToken);
        await using IChannel channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue: rabbitMqOptions.QueueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: rabbitMqOptions.QueueName,
            mandatory: false,
            body: JsonSerializer.SerializeToUtf8Bytes(message),
            cancellationToken: cancellationToken);
    }
}

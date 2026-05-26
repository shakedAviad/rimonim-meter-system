namespace MeterSystem.Messaging.Interfaces;

public interface IMessageProducer
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default);
}
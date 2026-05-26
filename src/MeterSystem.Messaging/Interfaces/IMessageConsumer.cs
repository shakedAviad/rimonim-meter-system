namespace MeterSystem.Messaging.Interfaces;

public interface IMessageConsumer
{
    Task ReceivedAsync<T>(Func<T, CancellationToken, Task> handleMessageAsync, CancellationToken cancellationToken = default);
}
using MeterSystem.Messaging.Interfaces;
using MeterSystem.Repositories.Interfaces;
using MeterSystem.Shared.Models;

namespace MeterSystem.Worker.Consumers;

internal class ReadingsConsumer(IMessageConsumer consumer, IRepository repository, ILogger<ReadingsConsumer> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await consumer.ReceivedAsync<MeterData>(
            async (message, cancellationToken) =>
            {
                await repository.SaveAsync(message, cancellationToken);

                logger.LogInformation("Persisted readings for meter {MeterNumber}", message.MeterNumber);
            },
            cancellationToken);

        await Task.Delay(Timeout.Infinite, cancellationToken);
    }
}
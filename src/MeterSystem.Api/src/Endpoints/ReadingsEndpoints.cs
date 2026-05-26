using MeterSystem.Api.Validation;
using MeterSystem.Messaging.Interfaces;
using MeterSystem.Shared.Models;

namespace MeterSystem.Api.Endpoints;

internal static class ReadingsEndpoints
{
    extension(WebApplication app)
    {
        public void MapReadings()
        {
            app.MapPost("/readings", async (MeterData request, IMeterReadingsRequestValidator validator, ILogger<Program> logger, IMessageProducer publisher, CancellationToken ct) =>
            {
                ValidationResult validationResult = validator.Validate(request);

                if (!validationResult.IsValid)
                {
                    logger.LogWarning("Validation failed for meter {MeterNumber}: {Errors}", request?.MeterNumber, string.Join("; ", validationResult.Errors));

                    return Results.BadRequest(validationResult.Errors);
                }

                await publisher.PublishAsync(request, ct);

                logger.LogInformation("Received readings for meter {MeterNumber} with {Count} readings", request.MeterNumber, request.Readings?.Count ?? 0);

                return Results.Accepted();

            }).WithReadingsMetadata();
        }
    }

    extension(RouteHandlerBuilder builder)
    {
        private RouteHandlerBuilder WithReadingsMetadata()
        {
            return builder.WithName("PostReadings")
                .Produces(StatusCodes.Status202Accepted)
                .Produces<IReadOnlyList<string>>(StatusCodes.Status400BadRequest);
        }
    }
}

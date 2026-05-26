using System.Text.Json;
using System.Text.Json.Nodes;
using MeterSystem.Api.Endpoints;
using MeterSystem.Api.Validation;
using MeterSystem.Messaging.Extensions;
using MeterSystem.Shared.Models;
using Scalar.AspNetCore;
namespace MeterSystem.Api.src;

public static class Extensions
{
    extension(WebApplicationBuilder builder)
    {
        public WebApplicationBuilder ConfigureServices()
        {
            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
                options.SerializerOptions.PropertyNameCaseInsensitive = true;
            });

            builder.Services.AddServices(builder.Configuration);

            return builder;
        }

        public WebApplication BuildApplication()
        {
            WebApplication app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();
            app.MapEndpoints();

            return app;
        }

    }
    extension(IServiceCollection services)
    {
        public IServiceCollection AddServices(IConfiguration configuration)
        {
            services.AddRabbitMqMessaging(configuration);
            services.AddSingleton<IMeterReadingsRequestValidator, MeterReadingsRequestValidator>();
            services.AddOpenApi(options =>
            {
                options.AddSchemaTransformer((schema, context, cancellationToken) =>
                {
                    if (context.JsonTypeInfo.Type == typeof(MeterData))
                    {
                        schema.Example = new JsonObject
                        {
                            ["meter_number"] = 1111111,
                            ["readings"] = new JsonObject
                            {
                                ["2026-04-25T11:30:00Z"] = 1000.17,
                                ["2028-01-02T08:45:00Z"] = 1000.94
                            }
                        };
                    }

                    return Task.CompletedTask;
                });
            }); ;

            return services;
        }
    }
    extension(WebApplication app)
    {
        public WebApplication MapEndpoints()
        {
            app.MapReadings();

            return app;
        }
    }
}

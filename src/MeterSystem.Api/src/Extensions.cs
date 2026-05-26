using System.Text.Json;
using MeterSystem.Api.Endpoints;
using MeterSystem.Api.Validation;
using MeterSystem.Messaging.Extensions;
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
            services.AddOpenApi();

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

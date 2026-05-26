using MeterSystem.Messaging.Extensions;
using MeterSystem.Repositories.Interfaces;
using MeterSystem.Worker.Consumers;
using MeterSystem.Worker.Repositories;

namespace MeterSystem.Worker.src;

public static class Extensions
{
    extension(HostApplicationBuilder builder)
    {
        public HostApplicationBuilder ConfigureServices()
        {
            builder.Services.AddServices(builder.Configuration);

            return builder;
        }

        public IHost BuildApplication()
        {
            return builder.Build();
        }
    }
    extension(IServiceCollection services)
    {
        public IServiceCollection AddServices(IConfiguration configuration)
        {
            services.AddRabbitMqMessaging(configuration);
            services.AddHostedService<ReadingsConsumer>();
            services.AddScoped<IRepository, MeterReadingsRepository>();

            return services;
        }
    }
}

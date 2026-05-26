using MeterSystem.Messaging.Interfaces;
using MeterSystem.Messaging.Providers.RabbitMq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MeterSystem.Messaging.Extensions;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddRabbitMqMessaging(IConfiguration configuration)
        {
            services.AddOptions<RabbitMqOptions>()
            .Bind(configuration.GetSection(RabbitMqOptions.SectionName))
            .ValidateOnStart();

            services.AddSingleton<IMessageProducer, RabbitMqMessageProducer>();
            services.AddSingleton<IMessageConsumer, RabbitMqMessageConsumer>();

            return services;
        }
    }
}
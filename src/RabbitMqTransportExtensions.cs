using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Whitestone.Cambion.Interfaces;

namespace Whitestone.Cambion.Transport.RabbitMQ
{
    public static class RabbitMqTransportExtensions
    {
        // ReSharper disable once InconsistentNaming
        public static ICambionSerializerBuilder UseRabbitMqTransport(this ICambionTransportBuilder builder, string connectionString)
        {
            return UseRabbitMqTransport(builder, conf => conf.Connection.ConnectionString = new Uri(connectionString));
        }

        public static ICambionSerializerBuilder UseRabbitMqTransport(this ICambionTransportBuilder builder, Action<RabbitMqConfig> configure)
        {
            return UseRabbitMqTransport(builder, null, configure, null);
        }

        public static ICambionSerializerBuilder UseRabbitMqTransport(this ICambionTransportBuilder builder, IConfiguration configuration, string cambionConfigurationKey = "Cambion")
        {
            return UseRabbitMqTransport(builder, configuration, null, cambionConfigurationKey);
        }

        // ReSharper disable once InconsistentNaming
        public static ICambionSerializerBuilder UseRabbitMqTransport(this ICambionTransportBuilder builder, IConfiguration configuration, Action<RabbitMqConfig> configure, string cambionConfigurationKey = "Cambion")
        {
            builder.Services.Replace(new ServiceDescriptor(typeof(ITransport), typeof(RabbitMqTransport), ServiceLifetime.Singleton));

            if (configuration != null)
            {
                string assemblyName = typeof(RabbitMqTransport).Assembly.GetName().Name;

                IConfigurationSection config = configuration.GetSection(cambionConfigurationKey).GetSection("Transport").GetSection(assemblyName);

                if (config.Exists())
                {
                    builder.Services.Configure<RabbitMqConfig>(config);
                }
            }

            builder.Services.AddOptions<RabbitMqConfig>()
                .Configure(conf => { configure?.Invoke(conf); });

            return (ICambionSerializerBuilder)builder;
        }

    }
}

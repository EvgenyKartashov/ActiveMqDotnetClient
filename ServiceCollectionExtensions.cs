using ActiveMqClient.Options;
using ActiveMqClient.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;


namespace ActiveMqClient
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection InitWorker(this IServiceCollection services, HostBuilderContext hostContext)
        {
            services.Configure<Queue1Config>(hostContext.Configuration.GetSection(Queue1Config.ConfigSection));
            services.Configure<Queue2Config>(hostContext.Configuration.GetSection(Queue2Config.ConfigSection));
            services.AddHostedService<MessageHostedService>();

            return services;
        }
    }
}
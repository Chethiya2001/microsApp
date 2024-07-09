using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Code.Settings;

namespace Platform.Code.MassTransit;

public static class Extention
{
    public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services)
    {
        services.AddMassTransit(congigure =>
        {
            congigure.AddConsumers(Assembly.GetEntryAssembly());
            congigure.UsingRabbitMq((context, cfg) =>
            {
                var configuration = context.GetService<IConfiguration>();
                var serviceSettings = configuration!.GetSection(nameof(ServiceSetting)).Get<ServiceSetting>();
                var rabbitMqSettings = configuration.GetSection(
                    nameof(RabbitMQSettings)).Get<RabbitMQSettings>();

                cfg.Host(rabbitMqSettings!.Host);
                //how the ques are created in rabbit mq
                cfg.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings!.ServiceName, false));
                cfg.UseMessageRetry(r =>
                {
                    r.Interval(3, TimeSpan.FromSeconds(5));
                }
                );
            });
        });

        return services;
    }
}

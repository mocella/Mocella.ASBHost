using Mocella.AsbHost.ServiceBus;
using Mocella.AsbHost.ServiceBus.Senders;
using Mocella.AsbHost.Services;

namespace Mocella.AsbHost.Configuration;

public static class ServiceRegistrationExtensions
{
    /// <summary>
    /// Registers all classes that inherit from <see cref="IServiceBusServiceBase"/> in the assembly of <typeparamref name="T"/> with the specified <see cref="ServiceLifetime"/>.
    /// </summary>
    /// <param name="webApplicationBuilder"></param>
    /// <param name="lifetime">DI Registration lifetime (Default: Singleton)</param>
    /// <typeparam name="T">Type in assembly being scanned for classes implementing <see cref="IServiceBusServiceBase"/></typeparam>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static void AddServiceBusServiceClasses<T>(this IHostApplicationBuilder webApplicationBuilder, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        webApplicationBuilder.Services.Scan(scan =>
        {
            var lifetimeSelector = scan
                .FromAssemblyOf<T>()
                .AddClasses(classes => classes.AssignableTo<IServiceBusServiceBase>())
                .AsSelf();

            switch (lifetime)
            {
                case ServiceLifetime.Scoped:
                    lifetimeSelector
                        .WithScopedLifetime();
                    break;
                case ServiceLifetime.Singleton:
                    lifetimeSelector
                        .WithSingletonLifetime();
                    break;
                case ServiceLifetime.Transient:
                    lifetimeSelector
                        .WithTransientLifetime();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
            }
        });
    }
    
    /// <summary>
    /// Registers all classes that inherit from <see cref="IMocellaService"/> in the assembly of <typeparamref name="T"/> with the specified <see cref="ServiceLifetime"/>.
    /// </summary>
    /// <param name="webApplicationBuilder"></param>
    /// <param name="lifetime">DI Registration lifetime (Default: Scoped)</param>
    /// <typeparam name="T">Type in assembly being scanned for classes extending IMocellaService</typeparam>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static void AddMocellaServiceClasses<T>(this IHostApplicationBuilder webApplicationBuilder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        webApplicationBuilder.Services.Scan(scan =>
        {
            var lifetimeSelector = scan
                .FromAssemblyOf<T>()
                .AddClasses(classes => classes.AssignableTo<IMocellaService>())
                .AsSelfWithInterfaces();

            switch (lifetime)
            {
                case ServiceLifetime.Scoped:
                    lifetimeSelector
                        .WithScopedLifetime();
                    break;
                case ServiceLifetime.Singleton:
                    lifetimeSelector
                        .WithSingletonLifetime();
                    break;
                case ServiceLifetime.Transient:
                    lifetimeSelector
                        .WithTransientLifetime();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
            }

        });
    }
    
    /// <summary>
    /// Registers all classes that implement <see cref="IServiceBusSenderBase"/> in the assembly of <typeparamref name="T"/> with the specified <see cref="ServiceLifetime"/>.
    /// </summary>
    /// <param name="webApplicationBuilder"></param>
    /// <param name="lifetime">DI Registration lifetime (Default: Singleton)</param>
    /// <typeparam name="T">Type in assembly being scanned for classes implementing IServiceBusSenderBase</typeparam>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static void AddServiceBusSenders<T>(this IHostApplicationBuilder webApplicationBuilder, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        webApplicationBuilder.Services.Scan(scan =>
        {
            var lifetimeSelector = scan
                .FromAssemblyOf<T>()
                .AddClasses(classes => classes.AssignableTo<IServiceBusSenderBase>())
                .AsImplementedInterfaces();

            switch (lifetime)
            {
                case ServiceLifetime.Scoped:
                    lifetimeSelector
                        .WithScopedLifetime();
                    break;
                case ServiceLifetime.Singleton:
                    lifetimeSelector
                        .WithSingletonLifetime();
                    break;
                case ServiceLifetime.Transient:
                    lifetimeSelector
                        .WithTransientLifetime();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
            }

        });
    }
}
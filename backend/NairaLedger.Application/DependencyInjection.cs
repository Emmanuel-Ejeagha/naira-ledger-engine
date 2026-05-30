using Microsoft.Extensions.DependencyInjection;
using NairaLedger.Application.Behaviors;
using System.Reflection;

namespace NairaLedger.Application;

/// <summary>
/// Registers all Application layer dependencies (MediatR, validators, pipeline behaviors).
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(IdempotencyBehavior<,>));
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
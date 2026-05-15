using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NairaWallet.Application.Behaviors;
using System.Reflection;

namespace NairaWallet.Application.Extensions;

/// <summary>
/// Registers all Application layer dependencies (MediatR, validators, pipeline behaviors).
/// </summary>
public static class ServiceCollectionExtensions
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
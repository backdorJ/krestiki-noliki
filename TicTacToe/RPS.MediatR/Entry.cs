using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace TicTacToe.MediatR;

public static class Entry
{
    public static void AddMediator(this IServiceCollection services, params Assembly[] handlersAssemblies)
    {
        if (handlersAssemblies == null || handlersAssemblies.Length == 0)
        {
            throw new ArgumentException("At least one assembly must be specified.");
        }

        var handlerTypes = handlersAssemblies.SelectMany(a => a.GetTypes()).ToList();

        var handlers = handlerTypes
            .Where(t => t.GetInterfaces().Any(i => 
                i.IsGenericType && 
                (i.GetGenericTypeDefinition() == typeof(IRequestHandler<>) ||
                 i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
            )).ToList();

        foreach (var handler in handlers)
        {
            foreach (var implementedInterface in handler.GetInterfaces())
            {
                if (implementedInterface.IsGenericType &&
                    (implementedInterface.GetGenericTypeDefinition() == typeof(IRequestHandler<>) ||
                     implementedInterface.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
                {
                    services.AddTransient(implementedInterface, handler);
                }
            }
        }

        services.AddScoped<IMediator, Mediator>();
    }
}
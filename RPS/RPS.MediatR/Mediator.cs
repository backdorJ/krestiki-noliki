using Microsoft.Extensions.DependencyInjection;

namespace TicTacToe.MediatR;

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));

        var handler = _serviceProvider.GetRequiredService(handlerType);
        var handleMethod = handler.GetType().GetMethod("Handle");

        if (handleMethod == null)
            throw new InvalidOperationException($"Handle method not found for {handlerType.Name}");

        return await (Task<TResponse>)handleMethod.Invoke(handler, new object[] { request, cancellationToken })!;
    }

    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
    {
        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<>).MakeGenericType(requestType);

        var handler = _serviceProvider.GetRequiredService(handlerType);
        var handleMethod = handler.GetType().GetMethod("Handle");

        if (handleMethod == null)
            throw new InvalidOperationException($"Handle method not found for {handlerType.Name}");

        return (Task)handleMethod.Invoke(handler, new object[] { request, cancellationToken })!;
    }

    public async Task<dynamic?> Send(dynamic request, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IRequestHandler<IRequest<dynamic>, dynamic>);
        var handler = _serviceProvider.GetService(handlerType) as IRequestHandler<IRequest<dynamic>, dynamic>;

        return handler != null 
            ? await handler.Handle(request, cancellationToken) 
            : null;
    }
}
namespace TicTacToe.MediatR;

public interface IRequest : IBaseRequest
{
}

public interface IRequest<out TResponse> : IBaseRequest
{
}

public interface IBaseRequest
{
}
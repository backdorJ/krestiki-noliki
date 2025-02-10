using Microsoft.EntityFrameworkCore;
using RPS.Core.Requests.Account.Register;
using TicTacToe.Core.Interfaces;
using TicTacToe.Domain.Entities;
using TicTacToe.MediatR;

namespace TicTacToe.Core.Requests.Account.Register;

public class RegisterCommandHandler(IDbContext dbContext) : IRequestHandler<RegisterCommand>
{
    public async Task Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var isExistUser = await dbContext.Users.AnyAsync(x => x.Name == request.Name, cancellationToken: cancellationToken);
        
        if (isExistUser)
            throw new ArgumentException("Username already taken");
        
        if (request.Password != request.PasswordConfirm)
            throw new ArgumentException("Passwords do not match");
        
        var user = new User
        {
            Name = request.Name,
            Password = request.Password
        };
        
        await dbContext.Users.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
﻿using Microsoft.EntityFrameworkCore;
using RPS.Core.Interfaces;
using TicTacToe.Core.Interfaces;
using TicTacToe.Core.Requests.Account.Login;
using TicTacToe.MediatR;

namespace RPS.Core.Requests.Account.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IDbContext _dbContext;
    private readonly IJwtGenerator _jwtGenerator;

    public LoginCommandHandler(IDbContext dbContext, IJwtGenerator jwtGenerator)
    {
        _dbContext = dbContext;
        _jwtGenerator = jwtGenerator;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            throw new ArgumentException("Username and/or password are required", nameof(request));
        
        var currentUser = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Name == request.Username, cancellationToken: cancellationToken)
            ?? throw new ArgumentException();
        
        if (currentUser.Password != request.Password)
            throw new ArgumentException("Passwords do not match", nameof(request));

        var token = _jwtGenerator.GenerateJwtToken(currentUser.Name, currentUser.Id);

        return new LoginResponse
        {
            JwtToken = token
        };
    }
}
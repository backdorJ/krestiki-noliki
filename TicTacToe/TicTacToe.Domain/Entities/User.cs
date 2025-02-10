﻿namespace TicTacToe.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; set; } = null!;

    public string Password { get; set; } = null!;

    public uint Rating { get; set; }

    public List<Game> Games { get; set; } = new List<Game>();
}
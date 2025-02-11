using System.Text;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RPS.Core;
using RPS.Core.Consumers;
using RPS.Core.Hubs;
using RPS.DAL;
using TicTacToe.Core;
using TicTacToe.DAL;
using TicTacToe.MediatR;
using Entry = RPS.Core.Entry;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddDal(builder.Configuration);
builder.Services.AddCore();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSignalR();
builder.Services.AddMediator(typeof(Program).Assembly, typeof(Entry).Assembly);

builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.SetKebabCaseEndpointNameFormatter();

    busConfigurator.AddConsumer<GameRatingConsumer>();
    busConfigurator.UsingRabbitMq((context, configurator) =>
    {
        configurator.ConfigureEndpoints(context);
        configurator.Host(builder.Configuration["rabbitmq"]);
    });
});


builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "TicTacToeApp",
            ValidAudience = builder.Configuration["Jwt:Issuer"] ?? "TicTacToeApp",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "super_secret_key_12345"))
        };
        
        // ВАЖНО: разрешаем аутентификацию через WebSocket
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                // Проверяем, что запрос направлен к SignalR-хабу
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/game-hub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

// Настройка CORS
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowAll", policy =>
            policy
                .AllowAnyOrigin()       // Разрешить любой источник (небезопасно для продакшн)
                .AllowAnyMethod()       // Разрешить любые методы
                .AllowAnyHeader()       // Разрешить любые заголовки
    );
});

var app = builder.Build();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var migrator = services.GetRequiredService<Migrator>();
await migrator.MigrateAsync();
// Подключаем CORS до маршрутизации
app.UseCors("AllowAll"); // Указываем имя политики явно

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.MapHub<GameHub>("/game-hub");

app.Run();
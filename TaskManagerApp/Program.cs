
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using TaskManagerApp.CommandHandlers;
using TaskManagerApp.Commands;
using TaskManagerApp.Queries;
using TaskManagerApp.QueryHandlers;
using TaskManagerApp.Repository;
using TaskManagerApp.Services;
using TaskManagerApp.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddSingleton<IMongoDatabase>(_ =>
    {
        var client = new MongoClient(builder
            .Configuration
            .GetSection("MongoDB:ConnectionUri").Value);
        return client.GetDatabase(builder.Configuration.GetSection("MongoDB:DatabaseName").Value);
    })
    .AddSingleton<SignalRClientHostedService>()
    .AddSingleton<ISignalRService, SignalRService>()
    .AddSingleton<ISignalRClientService, SignalRClientService>()
    .AddSingleton<IRabbitMqService, RabbitMqService>()
    .AddSingleton<IRepository, Repository>()
    .AddSingleton<IUserService, UserService>()
    .AddSingleton<IHandlerService, Handler>()
    .AddSingleton<IHandler<DeleteTaskCommand>, DeleteTaskCommandHandler>()
    .AddSingleton<IHandler<AddTaskCommand>, AddTaskCommandHandler>()
    .AddSingleton<IHandler<UpdateTaskCommand>, UpdateTaskCommandHandler>()
    .AddSingleton<IHandler<TasksQuery>, TasksQueryHandler>()
    .AddSingleton<ITaskService, TaskService>();

builder.Services.AddSignalR();
// builder.Services.AddHostedService<SignalRClientHostedService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(s =>
{
    s.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            Implicit = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri(builder
                    .Configuration
                    .GetSection("Keycloak:Host").Value + "/protocol/openid-connect/auth"),
                Scopes = { { "openid", "OpenID Connect" }, { "profile", "User Profile" }, { "email", "Email" } }
            }
        }
    });

    s.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                },
            },
            Array.Empty<string>() //scopes
        }
    });

});


builder.Services.AddAuthorization(o =>
{
    o.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireClaim("email_verified", "true")
        .Build();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<SignalRHub>("/socket");

app.Run();

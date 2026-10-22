using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RecSys.Application.Abstractions;
using RecSys.Infrastructure.BackgroundJobs;
using RecSys.Infrastructure.Caching;
using RecSys.Infrastructure.Messaging;
using RecSys.Infrastructure.Persistence;
using StackExchange.Redis;

namespace RecSys.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));

        services.AddSingleton<IConnection>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
            var factory = new ConnectionFactory
            {
                HostName = options.HostName,
                Port = options.Port,
                UserName = options.UserName,
                Password = options.Password
            };

            return factory.CreateConnectionAsync().GetAwaiter().GetResult();
        });

        services.AddSingleton<IInteractionEventQueue, RabbitMqInteractionEventQueue>();

        string passwordFile = Environment.GetEnvironmentVariable("DB_PASSWORD_FILE") ?? "secrets/db_password.txt";
        string password = File.Exists(passwordFile)
            ? File.ReadAllText(passwordFile).Trim()
            : configuration["Database:Password"] ?? "postgres";

        string host = configuration["Database:Host"] ?? "localhost";
        string port = configuration["Database:Port"] ?? "5432";
        string dbName = configuration["Database:Name"] ?? "recsys";
        string username = configuration["Database:Username"] ?? "postgres";

        string connectionString = configuration.GetConnectionString("Database")
            ?? $"Host={host};Port={port};Database={dbName};Username={username};Password={password}";

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString, o => o.UseVector()));

        services.AddSingleton<INpgSqlProvider>(_ => new NpgsqlProvider(connectionString));

        services.Configure<RedisOptions>(configuration.GetSection(RedisOptions.SectionName));
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var opt = sp.GetRequiredService<IOptions<RedisOptions>>().Value;
            return ConnectionMultiplexer.Connect(opt.ConnectionString);
        });
        services.AddSingleton<IRecommendationCacheService, RedisRecommendationCacheService>();
        services.AddHostedService<TasteProfileWorker>();

        return services;
    }
}

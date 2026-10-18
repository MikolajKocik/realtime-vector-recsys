namespace RecSys.Infrastructure.Messaging;

/// <summary>
/// Configuration options for connecting to RabbitMQ.
/// </summary>
public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";

    public string HostName { get; init; } = "localhost";
    public int Port { get; init; } = 5672;
    public string UserName { get; init; } = "guest";
    public string Password { get; init; } = "guest";
    public string ExchangeName { get; init; } = "recsys.interactions";
    public string QueueName { get; init; } = "user.interactions.queue";
    public string RoutingKey { get; init; } = "user.interacted";

    public string DlxExchangeName { get; init; } = "recsys.dlx";
    public string DlxQueueName { get; init; } = "dlx.queue";
    public string DlxRoutingKey { get; init; } = "dlx.route";
}

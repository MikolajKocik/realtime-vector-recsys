using System.Text.Json;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RecSys.Application.Abstractions;
using RecSys.Application.Features.Telemetry;

namespace RecSys.Infrastructure.Messaging;

/// <summary>
/// RabbitMQ implementation of <see cref="IInteractionEventQueue"/>.
/// Injects a shared singleton <see cref="IConnection"/> from DI.
/// Uses an event-driven push consumer paired with an internal async channel for zero-polling dequeue.
/// </summary>
public sealed class RabbitMqInteractionEventQueue : IInteractionEventQueue, IAsyncDisposable
{
    private readonly IConnection _connection;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqInteractionEventQueue> _logger;
    private readonly Channel<UserInteractedEvent> _internalChannel = Channel.CreateBounded<UserInteractedEvent>(
        new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = true
        });

    private IChannel? _channel;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private bool _isTopologyInitialized;

    public RabbitMqInteractionEventQueue(
        IConnection connection,
        IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqInteractionEventQueue> logger)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets or creates the RabbitMQ channel on the shared connection and ensures topology is initialized.
    /// </summary>
    private async Task<IChannel> GetChannelAsync(CancellationToken ct = default)
    {
        if (_channel is not null && _isTopologyInitialized)
        {
            return _channel;
        }

        await _initLock.WaitAsync(ct);
        try
        {
            if (_channel is not null && _isTopologyInitialized)
            {
                return _channel;
            }

            _channel ??= await _connection.CreateChannelAsync(cancellationToken: ct);

            if (!_isTopologyInitialized)
            {
                await InitializeTopologyAndConsumerAsync(_channel, ct);
                _isTopologyInitialized = true;
            }

            return _channel;
        }
        finally
        {
            _initLock.Release();
        }
    }

    /// <summary>
    /// Declares RabbitMQ exchanges, queues, DLX routing, and initializes the push consumer.
    /// </summary>
    private async Task InitializeTopologyAndConsumerAsync(IChannel channel, CancellationToken ct)
    {
        await channel.ExchangeDeclareAsync(
            exchange: _options.DlxExchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            cancellationToken: ct
        );

        await channel.QueueDeclareAsync(
            queue: _options.DlxQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: ct
        );

        await channel.QueueBindAsync(
            queue: _options.DlxQueueName,
            exchange: _options.DlxExchangeName,
            routingKey: _options.DlxRoutingKey,
            cancellationToken: ct
        );

        await channel.ExchangeDeclareAsync(
            exchange: _options.ExchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            cancellationToken: ct
        );

        var arguments = new Dictionary<string, object?>()
        {
            ["x-dead-letter-exchange"] = _options.DlxExchangeName,
            ["x-dead-letter-routing-key"] = _options.DlxRoutingKey
        };

        await channel.QueueDeclareAsync(
            queue: _options.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: arguments,
            cancellationToken: ct
        );

        await channel.QueueBindAsync(
            queue: _options.QueueName,
            exchange: _options.ExchangeName,
            routingKey: _options.RoutingKey,
            cancellationToken: ct
        );

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var @event = JsonSerializer.Deserialize<UserInteractedEvent>(ea.Body.Span);
                if (@event is null)
                {
                    await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                    return;
                }

                await _internalChannel.Writer.WriteAsync(@event);
                await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
            }
            catch (JsonException)
            {
                await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
            }
        };

        await channel.BasicConsumeAsync(
            queue: _options.QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: ct
        );
    }

    /// <summary>
    /// Publishes a user interaction event to the RabbitMQ exchange.
    /// </summary>
    public async ValueTask EnqueueAsync(UserInteractedEvent @event, CancellationToken ct = default)
    {
        var channel = await GetChannelAsync(ct);

        byte[] body = JsonSerializer.SerializeToUtf8Bytes(@event);

        var properties = new BasicProperties
        {
            DeliveryMode = DeliveryModes.Persistent,
            ContentType = "application/json"
        };

        await channel.BasicPublishAsync(
            exchange: _options.ExchangeName,
            routingKey: _options.RoutingKey,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: ct
        );
    }

    /// <summary>
    /// Asynchronously dequeues the next interaction event from the internal channel buffer.
    /// </summary>
    public async ValueTask<UserInteractedEvent> DequeueAsync(CancellationToken ct = default)
    {
        await GetChannelAsync(ct);
        return await _internalChannel.Reader.ReadAsync(ct);
    }

    /// <summary>
    /// Gracefully completes the internal channel and disposes RabbitMQ resources.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        _internalChannel.Writer.TryComplete();

        if (_channel is not null)
        {
            await _channel.CloseAsync();
            _channel.Dispose();
        }

        _initLock.Dispose();
    }
}

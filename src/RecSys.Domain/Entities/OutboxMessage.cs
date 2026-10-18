namespace RecSys.Domain.Entities;

/// <summary>
/// Represents an outbox message stored as part of the transactional outbox pattern.
/// Guarantees at-least-once message delivery to message brokers like RabbitMQ without dual-write inconsistency.
/// </summary>
public sealed class OutboxMessage
{
    public string Id { get; private set; }
    public string EventType { get; private set; }
    public string PayloadJson { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }
    public DateTimeOffset? ProcessedAt { get; private set; }
    public string? Error { get; private set; }

    private OutboxMessage() { }

    /// <summary>
    /// Initializes a new instance of an outbox message ready to be published.
    /// </summary>
    /// <param name="eventType">The type or routing key of the event (e.g. "UserInteracted").</param>
    /// <param name="payloadJson">The serialized JSON content of the domain event.</param>
    public OutboxMessage(string eventType, string payloadJson)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(payloadJson);
        ArgumentException.ThrowIfNullOrWhiteSpace(eventType);
        EventType = eventType;
        PayloadJson = payloadJson;

        Id = $"outmsg_{Guid.CreateVersion7()}";
        OccurredAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Marks the outbox message as successfully dispatched to the message broker.
    /// </summary>
    public void MarkAsProcessed()
    {
        ProcessedAt = DateTimeOffset.UtcNow;
        Error = null;
    }

    /// <summary>
    /// Records a dispatch failure to enable retry policies or dead-letter handling.
    /// </summary>
    public void MarkAsFailed(string error)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(error);
        Error = error;
    }
}

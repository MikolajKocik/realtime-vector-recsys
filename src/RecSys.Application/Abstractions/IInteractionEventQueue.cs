using RecSys.Application.Features.Telemetry;

namespace RecSys.Application.Abstractions;

/// <summary>
/// Provides asynchronous queue abstraction for publishing and consuming telemetry interaction events.
/// </summary>
public interface IInteractionEventQueue
{
    /// <summary>
    /// Enqueues a user interaction event to the message broker or in-memory channel.
    /// </summary>
    /// <param name="event">The user interaction telemetry event.</param>
    /// <param name="ct">Cancellation token.</param>
    ValueTask EnqueueAsync(UserInteractedEvent @event, CancellationToken ct = default);

    /// <summary>
    /// Asynchronously dequeues the next user interaction event for background processing.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The dequeued interaction event.</returns>
    ValueTask<UserInteractedEvent> DequeueAsync(CancellationToken ct = default);
}

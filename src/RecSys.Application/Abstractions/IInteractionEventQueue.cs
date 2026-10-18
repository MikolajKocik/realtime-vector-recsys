using RecSys.Application.Features.Telemetry;

namespace RecSys.Application.Abstractions;

public interface IInteractionEventQueue
{
    ValueTask EnqueueAsync(UserInteractedEvent @event, CancellationToken ct = default);
    ValueTask<UserInteractedEvent> DequeueAsync(CancellationToken ct = default);
}
